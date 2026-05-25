//using System.Collections.Generic;
//using UnityEngine;

//public class InventoryUIMapper : MonoBehaviour
//{
//    [SerializeField] private InventoryUI inventoryUI;

//    public CharacterInventory CharacterInven {  get; private set; }

//    public bool TryGetMappingItemSlot(int uiSlot, out ItemId itemId, out int slotIndex)
//    {
//        itemId = default;
//        slotIndex = -1;
//        if (uiSlotToInvenSlot.TryGetValue(uiSlot, out int s))
//        {
//            itemId = CharacterInven.Inventory.GetItem(s);
//            slotIndex = s;
//            return true;
//        }
//        return false;
//    }

//    public void Initalize(CharacterInventory characterInventory)
//    {
//        CharacterInven = characterInventory;

//        CharacterInven.Inventory.OnSlotUpdate += (int slotIndex, ItemId itemId, int count) =>
//        {
//            ItemData item = CharacterInven.ItemManager.Get(itemId);
//            if (invenSlotToUISlot.TryGetValue(slotIndex, out int uiSlot))
//                inventoryUI.UpdateItemUI(uiSlot, item.Name, item.Icon, count);
//            else
//            {
//                int newUISlot = inventoryUI.AddItemUI(item.Name, item.Icon, count);
//                invenSlotToUISlot[slotIndex] = newUISlot;
//                uiSlotToInvenSlot[newUISlot] = slotIndex;
//            }
//        };

//        CharacterInven.Inventory.OnSlotRemove += (int slotIndex, ItemId itemId) =>
//        {
//            Debug.Assert(invenSlotToUISlot.TryGetValue(slotIndex, out int uiSlot));

//            inventoryUI.RemoveItem(uiSlot);
//            uiSlotToInvenSlot.Remove(uiSlot);
//            invenSlotToUISlot.Remove(slotIndex);
//        };
//    }

//    public void SwapSlot(int uiSlotA, int uiSlotB)
//    {
//        if (uiSlotA == uiSlotB)
//            return;

//        inventoryUI.SwapItemUI(uiSlotA, uiSlotB);

//        uiSlotToInvenSlot.TryGetValue(uiSlotA, out int invenSlotA);
//        uiSlotToInvenSlot.TryGetValue(uiSlotB, out int invenSlotB);

//        bool hasA = uiSlotToInvenSlot.ContainsKey(uiSlotA);
//        bool hasB = uiSlotToInvenSlot.ContainsKey(uiSlotB);

//        if (hasA) uiSlotToInvenSlot[uiSlotB] = invenSlotA;
//        else uiSlotToInvenSlot.Remove(uiSlotB);

//        if (hasB) uiSlotToInvenSlot[uiSlotA] = invenSlotB;
//        else uiSlotToInvenSlot.Remove(uiSlotA);

//        if (hasA) invenSlotToUISlot[invenSlotA] = uiSlotB;
//        if (hasB) invenSlotToUISlot[invenSlotB] = uiSlotA;
//    }

//    public InventoryUI LinkedInventoryUI { get { return inventoryUI; } }

//    private Dictionary<int, int> invenSlotToUISlot = new();
//    private Dictionary<int, int> uiSlotToInvenSlot = new();
//}
