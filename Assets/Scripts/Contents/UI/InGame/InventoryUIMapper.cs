using System.Collections.Generic;
using UnityEngine;

public class InventoryUIMapper : MonoBehaviour
{
    private InventoryUI inventoryUI;

    private ItemLoader itemLoader;
    private readonly Dictionary<int, int> _itemIdToUIIndex = new Dictionary<int, int>();

    public void Initialize(InventoryUI inventoryUI, ItemLoader itemLoader)
    {
        Debug.Assert(inventoryUI, "InventoryUIController: InventoryUI가 연결되지 않았습니다.");

        this.inventoryUI = inventoryUI;
        this.itemLoader = itemLoader;
    }
    public void OnItemAdd(int itemId, int count, int maxCount)
    {
        ItemData itemData = itemLoader.GetItemData<ItemData>(itemId);
        int index = inventoryUI.AddItemUI(itemData.itemName, itemData.icon, count);
        _itemIdToUIIndex.Add(itemId, index);
    }

    public void OnItemSub(int itemId, int count, int maxCount)
    {
        ItemData itemData = itemLoader.GetItemData<ItemData>(itemId);
        Debug.Assert(_itemIdToUIIndex.TryGetValue(itemId, out int index));
        inventoryUI.UpdateItemUI(index, itemData.itemName, itemData.icon, count);
    }

    public void OnItemRemove(int itemId)
    {
        Debug.Assert(_itemIdToUIIndex.TryGetValue(itemId, out int index));
        inventoryUI.RemoveItem(index);
        _itemIdToUIIndex.Remove(itemId);
    }
}
