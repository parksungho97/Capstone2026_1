using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemHotKey : HotKeySlot
{
    public ItemHotKey(Item item, ItemUser itemUser)
    {
        Name = item.Name;
        Icon = item.Icon;
        this.item = item;
        this.itemUser = itemUser;
    }
    public override void Execute()
    {
        itemUser.UseItem(item);
    }

    private Item item;
    private ItemUser itemUser;
}

public class InvenHotKeyMapper : MonoBehaviour
{
    [SerializeField] private InventoryUIMapper inventoryUIMapper;
    [SerializeField] private HotKey hotKey;
    [SerializeField] private ItemUser itemUser;

    private void Start()
    {
        inventoryUIMapper.LinkedInventoryUI.ActionDisabled += Clear;

        inventoryUIMapper.LinkedInventoryUI.ActionSlotClicked += (int uiSlot) =>
        {
            Item item = null;
            int slotIndex = -1;

            inventoryUIMapper.TryGetMappingItemSlot(uiSlot, out item, out slotIndex);

            if (putItem != null)
            {
                inventoryUIMapper.SwapSlot(putUISlot, uiSlot);
                Clear();
            }
            else
            {
                putItem = item;
                putUISlot = uiSlot;
            }
        };

        hotKey.ActionHotKeyClick += (EHotKeyType hotKeyType) =>
        {
            if(putItem != null)
            {
                Debug.Log("HotKeyClicked");
                ItemHotKey itemHotKey = new ItemHotKey(putItem, itemUser);
                hotKey.SetSlot(hotKeyType, itemHotKey);

                itemHotKeyMappings.Add(putItem, hotKeyType);
                Clear();
            }
            else
            {
                if (prevHotKeyType == EHotKeyType.Error)
                    prevHotKeyType = hotKeyType;
                else
                {
                    hotKey.SwapSlot(prevHotKeyType, hotKeyType);
                    Clear();
                }
            }
        };

        inventoryUIMapper.LinkedInventory.OnSlotRemove += (int slotIndex, Item item) =>
        {
            if (itemHotKeyMappings.TryGetValue(item, out EHotKeyType hotKeyType))
            {
                hotKey.SetSlot(hotKeyType, null);
                itemHotKeyMappings.Remove(item);
            }
        };
    }

    private void Clear()
    {
        putItem = null;
        putUISlot = -1;
        prevHotKeyType = EHotKeyType.Error;
    }

    private Item putItem = null;
    private int putUISlot = -1;

    private EHotKeyType prevHotKeyType = EHotKeyType.Error;

    private Dictionary<Item, EHotKeyType> itemHotKeyMappings = new();

}
