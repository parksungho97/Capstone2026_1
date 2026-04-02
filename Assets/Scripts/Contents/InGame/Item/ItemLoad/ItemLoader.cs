using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Usable,
}

public class ItemData
{
    public ItemData(int id, string itemName, string description, Sprite icon, EItemType itemType, int maxCount = 1)
    {
        this.id = id;
        this.itemName = itemName;
        this.description = description;
        this.icon = icon;
        this.itemType = itemType;
        this.maxCount = maxCount;
    }
    public int id;
    public string itemName;
    public string description;
    public Sprite icon;
    public EItemType itemType;
    public int maxCount;
}

public class ItemLoader
{
    private static ItemLoader instance;
    public static ItemLoader Instance
    {
        get
        {
            if (instance == null)
                instance = new ItemLoader();
            return instance;
        }
    }

    public void LoadItemData(ItemData itemLoadData)
    {
        if(itemDatas.ContainsKey(itemLoadData.id))
        {
            Debug.LogError($"ItemLoader: Item with id {itemLoadData.id} already exists.");
            return;
        }
        itemDatas.Add(itemLoadData.id, itemLoadData);
    }

    public T GetItemData<T>(int id) where T : ItemData
    {
        if (!itemDatas.TryGetValue(id, out ItemData itemData))
        {
            Debug.LogError($"ItemLoader: Item with id {id} does not exist.");
            return null;
        }

        if (itemData is T result)
            return result;

        return null;
    }

    private Dictionary<int, ItemData> itemDatas = new Dictionary<int, ItemData>();
}
