using UnityEngine;

public class InventoryEquipBinder : MonoBehaviour
{
    [SerializeField] private InventoryUIMapper inventoryUIMapper;

    private void Awake()
    {
        inventoryUIMapper.InventoryUI.ActionSlotDoubleClicked += OnSlotDoubleClicked;
    }

    private void OnSlotDoubleClicked(int uiSlotIndex)
    {
        if (!inventoryUIMapper.TryGetEquipmentByUiSlot(uiSlotIndex, out int storeSlotIndex))
            return;

        GameObject userObj = inventoryUIMapper.UserGameObject;
        EquipmentStore equipmentStore = userObj.GetComponent<EquipmentStore>();
        Debug.Assert(equipmentStore);

        Equipment equipment = equipmentStore.Get<Equipment>(storeSlotIndex);
        if (equipment == null) return;

        if (equipment is Helmet helmet)
        {
            EquipmentSlot equipmentSlot = userObj.GetComponent<EquipmentSlot>();
            Debug.Assert(equipmentSlot);

            if (equipmentSlot.Helmet != null)
                equipmentStore.AddEquip(equipmentSlot.Helmet);

            equipmentStore.Remove(storeSlotIndex);
            equipmentSlot.SetHelmet(helmet);
        }
        else if (equipment is Weapon)
        {
            ExtraWeaponSlot extraWeaponSlot = userObj.GetComponent<ExtraWeaponSlot>();
            Debug.Assert(extraWeaponSlot);
            extraWeaponSlot.EquipFromStore(storeSlotIndex);
        }
    }
}
