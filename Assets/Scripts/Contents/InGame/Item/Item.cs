using Fusion;
using UnityEngine;

public class ItemData
{
    public ItemData(string name, Sprite icon, string desc, GameObject itemPrefab, int spawnCount = 1)
    {
        Name = name;
        Icon = icon;
        Desc = desc;
        ItemPrefab = itemPrefab;
        SpawnCount = Mathf.Max(1, spawnCount);
    }

    public string Name { get; }
    public Sprite Icon { get; }
    public string Desc { get; }
    public GameObject ItemPrefab { get; }
    public int SpawnCount { get; }
}
