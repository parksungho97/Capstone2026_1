using System;
using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    private void Start()
    {
        equipmentStore = GetComponent<EquipmentStore>();
    }

    public Action<Weapon> ActionWeaponEquip;
    public Helmet Helmet { get; private set; }
    public Weapon Weapon { get; private set; }
    public int WeaponId => Weapon?.Id ?? -1;

    public void SetHelmet(Helmet helmet) => Helmet = helmet;
    public void SetWeapon(Weapon weapon)
    {
        Weapon = weapon;
        ActionWeaponEquip?.Invoke(Weapon);
    }

    public void Equip(int slot)
    {
        Equipment equipment = equipmentStore.Get<Equipment>(slot);
        if (equipment == null) return;

        if (equipment is Helmet helmet)
        {
            if (Helmet != null) equipmentStore.AddEquip(Helmet);
            SetHelmet(helmet);
        }
        else if (equipment is Weapon weapon)
        {
            if (Weapon != null) equipmentStore.AddEquip(Weapon);
            SetWeapon(weapon);
        }
        else return;

        equipmentStore.Remove(slot);
    }

    public void Unequip(EquipmentType type, EquipmentStore store)
    {
        switch (type)
        {
            case EquipmentType.Helmet:
                if (Helmet == null) return;
                store.AddEquip(Helmet);
                SetHelmet(null);
                break;
            case EquipmentType.Weapon:
                if (Weapon == null) return;
                store.AddEquip(Weapon);
                SetWeapon(null);
                break;
        }
    }

    private EquipmentStore equipmentStore;
}
