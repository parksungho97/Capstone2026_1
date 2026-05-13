using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIMapper : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUI inventoryUI;
    
    public bool TryGetMappingItemSlot(int uiSlot, out Item item, out int slotIndex)
    {
        item = null;
        slotIndex = -1;
        if (uiSlotToInvenSlot.TryGetValue(uiSlot, out int s))
        {
            item = LinkedInventory.GetItem(s);
            slotIndex = s;
            return true;
        }
        return false;
    }

    private void Start()
    {
        Debug.Assert(inventory);
        Debug.Assert(inventoryUI);

        LinkedInventory.OnSlotUpdate += (int slotIndex, Item item, int count) =>
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

        LinkedInventory.OnSlotRemove += (int slotIndex, Item item) =>
        {
            Debug.Assert(invenSlotToUISlot.TryGetValue(slotIndex, out int uiSlot));

            inventoryUI.RemoveItem(uiSlot);
            uiSlotToInvenSlot.Remove(uiSlot);
            invenSlotToUISlot.Remove(slotIndex);
        };
    }

    public void SwapSlot(int uiSlotA, int uiSlotB)
    {
        if (uiSlotA == uiSlotB)
            return;

        inventoryUI.SwapItemUI(uiSlotA, uiSlotB);

        // uiSlot → invenSlot 스왑
        uiSlotToInvenSlot.TryGetValue(uiSlotA, out int invenSlotA);
        uiSlotToInvenSlot.TryGetValue(uiSlotB, out int invenSlotB);

        bool hasA = uiSlotToInvenSlot.ContainsKey(uiSlotA);
        bool hasB = uiSlotToInvenSlot.ContainsKey(uiSlotB);

        if (hasA) uiSlotToInvenSlot[uiSlotB] = invenSlotA;
        else uiSlotToInvenSlot.Remove(uiSlotB);

        if (hasB) uiSlotToInvenSlot[uiSlotA] = invenSlotB;
        else uiSlotToInvenSlot.Remove(uiSlotA);

        // invenSlot → uiSlot 스왑
        if (hasA) invenSlotToUISlot[invenSlotA] = uiSlotB;
        if (hasB) invenSlotToUISlot[invenSlotB] = uiSlotA;
    }

    public Inventory LinkedInventory { get { return inventory; } }
    public InventoryUI LinkedInventoryUI { get { return inventoryUI; } }

    private Dictionary<int, int> invenSlotToUISlot = new();
    private Dictionary<int, int> uiSlotToInvenSlot = new();
}