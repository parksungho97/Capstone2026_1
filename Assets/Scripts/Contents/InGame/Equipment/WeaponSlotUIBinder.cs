using UnityEngine;

public class WeaponSlotUIBinder : MonoBehaviour
{
    [SerializeField] private WeaponSlotUI weaponSlotUI;

    private EquipmentSlot equipmentSlot;
    private ConsumptionStore consumptionStore;
    private int trackedConsumptionId = -1;

    public void Link(EquipmentSlot slot)
    {
        if (equipmentSlot != null)
            equipmentSlot.ActionWeaponEquip -= OnWeaponEquipped;

        if (consumptionStore != null)
        {
            consumptionStore.OnItemAdded   -= OnAmmoItemAdded;
            consumptionStore.OnCountChanged -= OnAmmoCountChanged;
            consumptionStore.OnItemRemoved  -= OnAmmoItemRemoved;
        }

        equipmentSlot    = slot;
        consumptionStore = slot.GetComponent<ConsumptionStore>();

        equipmentSlot.ActionWeaponEquip += OnWeaponEquipped;

        if (consumptionStore != null)
        {
            consumptionStore.OnItemAdded    += OnAmmoItemAdded;
            consumptionStore.OnCountChanged += OnAmmoCountChanged;
            consumptionStore.OnItemRemoved  += OnAmmoItemRemoved;
        }

        OnWeaponEquipped(equipmentSlot.Weapon);
    }

    private void OnDestroy()
    {
        if (equipmentSlot != null)
            equipmentSlot.ActionWeaponEquip -= OnWeaponEquipped;

        if (consumptionStore != null)
        {
            consumptionStore.OnItemAdded    -= OnAmmoItemAdded;
            consumptionStore.OnCountChanged -= OnAmmoCountChanged;
            consumptionStore.OnItemRemoved  -= OnAmmoItemRemoved;
        }
    }

    private void OnWeaponEquipped(Weapon weapon)
    {
        trackedConsumptionId = -1;

        if (weapon == null)
        {
            weaponSlotUI.SetWeapon(string.Empty, null, WeaponAmmoType.Infinite, 0, 0);
            return;
        }

        WeaponAmmoType ammoType = weapon.AttackType == EAttackType.Melee
            ? WeaponAmmoType.Infinite
            : WeaponAmmoType.Limited;

        int count = 0, maxCount = 0;
        if (ammoType == WeaponAmmoType.Limited && TryGetConsumptionId(weapon, out int consumptionId))
        {
            trackedConsumptionId = consumptionId;
            count = consumptionStore != null ? consumptionStore.GetCount(consumptionId) : 0;
            if (ConsumptionManager.Instance.TryGet(consumptionId, out ConsumptionData data))
                maxCount = data.MaxCount;
        }

        if (ItemMappings.Instance.TryGetItemByTypeValue(EItemMapType.Equipment, weapon.Id, out ItemId itemId))
        {
            ItemData itemData = ItemManager.Instance.Get(itemId);
            if (itemData != null)
            {
                weaponSlotUI.SetWeapon(itemData.Name, itemData.Icon, ammoType, count, maxCount);
                return;
            }
        }

        weaponSlotUI.SetWeapon(weapon.Id.ToString(), null, ammoType, count, maxCount);
    }

    // Fired when a brand-new ammo type is added (count 0 → first stack)
    private void OnAmmoItemAdded()
    {
        if (trackedConsumptionId < 0 || consumptionStore == null) return;
        RefreshAmmoUI();
    }

    // Fired when an existing ammo count changes
    private void OnAmmoCountChanged(int consumptionId, int newCount)
    {
        if (consumptionId != trackedConsumptionId) return;
        int maxCount = GetMaxCount(consumptionId);
        weaponSlotUI.UpdateAmmo(newCount, maxCount);
    }

    // Fired when ammo count reaches 0 and entry is removed
    private void OnAmmoItemRemoved(int consumptionId)
    {
        if (consumptionId != trackedConsumptionId) return;
        int maxCount = GetMaxCount(consumptionId);
        weaponSlotUI.UpdateAmmo(0, maxCount);
    }

    private void RefreshAmmoUI()
    {
        int count    = consumptionStore.GetCount(trackedConsumptionId);
        int maxCount = GetMaxCount(trackedConsumptionId);
        weaponSlotUI.UpdateAmmo(count, maxCount);
    }

    private int GetMaxCount(int consumptionId)
    {
        return ConsumptionManager.Instance.TryGet(consumptionId, out ConsumptionData data)
            ? data.MaxCount
            : 0;
    }

    private bool TryGetConsumptionId(Weapon weapon, out int consumptionId)
    {
        foreach (int attackId in weapon.AttackIds)
        {
            if (AttackConsumptionMapping.Instance.TryGetConsumption(attackId, out consumptionId))
                return true;
        }
        consumptionId = -1;
        return false;
    }
}
