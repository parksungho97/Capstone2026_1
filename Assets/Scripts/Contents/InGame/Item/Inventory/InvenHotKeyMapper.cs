using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        inventoryUIMapper.ActionItemClicked += (Item item) =>
        {
            Debug.Log("ItemClicked");
            putItem = item;
        };

        hotKey.ActionHotKeyClick += (EHotKeyType hotKeyType) =>
        {
            if(putItem != null)
            {
                Debug.Log("HotKeyClicked");
                ItemHotKey itemHotKey = new ItemHotKey(putItem, itemUser);
                hotKey.SetSlot(hotKeyType, itemHotKey);
                putItem = null;
            }
        };
    }

    private void Update()
    {
    }

    private Item putItem = null;
}
