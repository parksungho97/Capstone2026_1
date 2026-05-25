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

        ExtraWeaponSlot extraWeaponSlot = inventoryUIMapper.UserGameObject.GetComponent<ExtraWeaponSlot>();
        Debug.Assert(extraWeaponSlot);

        extraWeaponSlot.EquipFromStore(storeSlotIndex);
    }
}
