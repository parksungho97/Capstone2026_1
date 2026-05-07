using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHotKey : HotKeySlot
{
    public ItemHotKey(Item item, ItemUseSystem itemUseSystem, GameObject gameObject)
    {
        Name = item.Name;
        Icon = item.Icon;
        this.item = item;
        this.itemUseSystem = itemUseSystem;
        this.gameObject = gameObject;
    }
    public override void Execute()
    {
        //itemUseSystem.Use(item, gameObject);
    }

    private Item item;
    private ItemUseSystem itemUseSystem;
    private GameObject gameObject;
}

public class InvenHotKeyMapper : MonoBehaviour
{
    [SerializeField] private InventoryUIMapper inventoryUIMapper;
    [SerializeField] private HotKey hotKey;

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
                ItemHotKey itemHotKey = new ItemHotKey(putItem, null, null);
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
