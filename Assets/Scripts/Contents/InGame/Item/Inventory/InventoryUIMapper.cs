using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIMapper : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    public Action<Item> ActionItemClicked;
    public void LinkInventory(Inventory inven)
    {
        Debug.Assert(inventoryUI);

        inventory = inven;

        inventory.OnSlotUpdate += (int slotIndex, Item item, int count) =>
        {
            if (invenSlotToUISlot.TryGetValue(slotIndex, out int uiSlot))
                inventoryUI.UpdateItemUI(uiSlot, item.Name, item.Icon, count);
            else
            {
                int newUISlot = inventoryUI.AddItemUI(item.Name, item.Icon, count);
                invenSlotToUISlot[slotIndex] = newUISlot;
                uiSlotToInvenSlot[newUISlot] = slotIndex;
            }
        };

        inventory.OnSlotRemove += (int slotIndex) =>
        {
            Debug.Assert(invenSlotToUISlot.TryGetValue(slotIndex, out int uiSlot));

            inventoryUI.RemoveItem(uiSlot);
            uiSlotToInvenSlot.Remove(uiSlot);
            invenSlotToUISlot.Remove(slotIndex);
        };

        inventoryUI.ActionSlotClicked += (int uiSlot) =>
        {
            if (uiSlotToInvenSlot.TryGetValue(uiSlot, out int slotIndex) == false)
                return;

            Item item = inventory.GetItem(slotIndex);
            ActionItemClicked?.Invoke(item);
        };
    }

    private void Start()
    {
        Debug.Assert(inventoryUI);
    }

    private Inventory inventory;

    private Dictionary<int, int> invenSlotToUISlot = new();
    private Dictionary<int, int> uiSlotToInvenSlot = new();
}