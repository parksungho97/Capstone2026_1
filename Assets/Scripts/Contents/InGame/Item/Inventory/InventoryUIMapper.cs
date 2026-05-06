using System.Collections.Generic;
using UnityEngine;

public class InventoryUIMapper : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    public void LinkInventory(Inventory inven)
    {
        Debug.Assert(inventoryUI);

        inventory = inven;

        inventory.OnSlotUpdate += (int slotIndex, Item item, int count) =>
        {
            if(invenSlotToUISlotMappings.TryGetValue(slotIndex, out int UISlot))
                inventoryUI.UpdateItemUI(UISlot, item.Name, item.Icon, count);
            else
            {
                int UiSlot = inventoryUI.AddItemUI(item.Name, item.Icon, count);
                invenSlotToUISlotMappings[slotIndex] = UiSlot;
            }
        };

        inventory.OnSlotRemove += (int slotIndex) =>
        {
            Debug.Assert(invenSlotToUISlotMappings.TryGetValue(slotIndex, out int UISlot));

            inventoryUI.RemoveItem(UISlot);
            invenSlotToUISlotMappings.Remove(slotIndex);
        };
    }
    private void Start()
    {
        Debug.Assert(inventoryUI);
    }

    private Inventory inventory;

    private Dictionary<int, int> invenSlotToUISlotMappings = new Dictionary<int, int>();
}
