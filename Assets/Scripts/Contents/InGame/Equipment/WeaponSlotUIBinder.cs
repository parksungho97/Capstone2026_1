using UnityEngine;

public class WeaponSlotUIBinder : MonoBehaviour
{
    [SerializeField] private WeaponSlotUI weaponSlotUI;

    private EquipmentSlot equipmentSlot;

    public void Link(EquipmentSlot slot)
    {
        if (equipmentSlot != null)
            equipmentSlot.ActionWeaponEquip -= OnWeaponEquipped;

        equipmentSlot = slot;
        equipmentSlot.ActionWeaponEquip += OnWeaponEquipped;

        OnWeaponEquipped(equipmentSlot.Weapon);
    }

    private void OnWeaponEquipped(Weapon weapon)
    {
        if (weapon == null)
        {
            weaponSlotUI.SetWeapon(string.Empty, null);
            return;
        }

        if (ItemMappings.Instance.TryGetItemByTypeValue(EItemMapType.Equipment, weapon.Id, out ItemId itemId))
        {
            ItemData data = ItemManager.Instance.Get(itemId);
            if (data != null)
            {
                weaponSlotUI.SetWeapon(data.Name, data.Icon);
                return;
            }
        }

        weaponSlotUI.SetWeapon(weapon.Id.ToString(), null);
    }
}
