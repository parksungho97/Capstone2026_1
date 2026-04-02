using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemLoadData : ScriptableObject
{
    public int id;
    public string itemName;
    public string description;
    public Sprite icon;
    public int maxCount;
    public EItemType type;

    public abstract ItemData ToItemData();
}

public class ItemSystem : MonoBehaviour
{
    [SerializeField] private ItemLoadData[] itemSOList;
    private void Start()
    {
        itemLoader = ItemLoader.Instance;
        itemUseManager = ItemUseManager.Instance;
        itemUseManager.Initalize();

        foreach (var itemSO in itemSOList)
        {
            ItemData itemData = itemSO.ToItemData();
            itemLoader.LoadItemData(itemData);
        }
    }
    private ItemLoader itemLoader;
    private ItemUseManager itemUseManager;
}
