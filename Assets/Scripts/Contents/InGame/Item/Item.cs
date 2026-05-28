using Fusion;
using UnityEngine;

public class ItemData
{
    public ItemData(string name, Sprite icon, string desc, GameObject itemPrefab)
    {
        Name = name;
        Icon = icon;
        Desc = desc;
        ItemPrefab = itemPrefab;
    }

    public string Name { get; }
    public Sprite Icon { get; }
    public string Desc { get; }
    public GameObject ItemPrefab { get; }
}
