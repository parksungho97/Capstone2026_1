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
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    public void Load(ItemManager itemManager, ItemMappings itemMappings)
    {
        itemManager.Register(itemId, new ItemData(itemName, icon, desc, mesh, material));
        itemMappings.Register(itemId, type, typeValue);
    }
}
