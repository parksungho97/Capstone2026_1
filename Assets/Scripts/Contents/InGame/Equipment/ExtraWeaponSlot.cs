using UnityEngine;

public class ExtraWeaponSlot : MonoBehaviour
{
    [SerializeField] private int defaultWeaponId = 0;

    public Weapon Weapon { get; private set; }

    private EquipmentSlot equipmentSlot;
    private EquipmentStore equipmentStore;

    private void Start()
    {
        equipmentSlot = GetComponent<EquipmentSlot>();
        equipmentStore = GetComponent<EquipmentStore>();
        Debug.Assert(equipmentSlot);
        Debug.Assert(equipmentStore);
    }

    public void EquipFromStore(int storeSlotIndex)
    {
        Weapon weapon = equipmentStore.Get<Weapon>(storeSlotIndex);
        if (weapon == null) return;
        equipmentStore.Remove(storeSlotIndex);
        Equip(weapon);
    }

    public void Swap()
    {
        if (Weapon != null)
        {
            Weapon temp = equipmentSlot.Weapon;
            equipmentSlot.SetWeapon(Weapon);
            Weapon = temp;
        }
    }

    public void Equip(Weapon weapon)
    {
        bool defaultInSlot = equipmentSlot.Weapon?.Id == defaultWeaponId;

        if (defaultInSlot)
        {
            if (Weapon != null)
                equipmentStore.AddEquip(Weapon);
            Weapon = weapon;
        }
        else
        {
            if (equipmentSlot.Weapon != null)
                equipmentStore.AddEquip(equipmentSlot.Weapon);
            equipmentSlot.SetWeapon(weapon);
        }
    }
}
