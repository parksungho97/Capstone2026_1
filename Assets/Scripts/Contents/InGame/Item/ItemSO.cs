using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Items/Item")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private ItemId itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private string desc;
    [SerializeField] private EItemMapType type;
    [SerializeField] private int typeValue;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int spawnCount = 1;

    public void Load(ItemManager itemManager, ItemMappings itemMappings)
    {
        itemManager.Register(itemId, new ItemData(itemName, icon, desc, itemPrefab, spawnCount));
        itemMappings.Register(itemId, type, typeValue);
    }
}
