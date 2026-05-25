using UnityEngine;

public class ExtraWeaponSlot : MonoBehaviour
{
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
        Weapon temp = equipmentSlot.Weapon;
        equipmentSlot.SetWeapon(Weapon);
        Weapon = temp;
    }

    public void Equip(Weapon weapon)
    {
        if (equipmentSlot.Weapon == null)
            equipmentSlot.SetWeapon(weapon);
        else
            Weapon = weapon;
    }
}
