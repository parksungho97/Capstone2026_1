using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemId : System.IEquatable<ItemId>
{
    public int Value;

    public ItemId(int value) { Value = value; }

    public bool Equals(ItemId other) => Value == other.Value;
    public override bool Equals(object obj) => obj is ItemId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(ItemId a, ItemId b) => a.Value == b.Value;
    public static bool operator !=(ItemId a, ItemId b) => a.Value != b.Value;
    public override string ToString() => Value.ToString();
}


public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField] private ItemMappings itemMappings;
    [SerializeField] private List<ItemSO> itemSOs;

    private Dictionary<ItemId, ItemData> map = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var so in itemSOs)
            so.Load(this, itemMappings);
    }

    public void Register(ItemId id, ItemData data)
    {
        map[id] = data;
    }

    public bool TryGet(ItemId id, out ItemData data)
        => map.TryGetValue(id, out data);

    public ItemData Get(ItemId id)
    {
        TryGet(id, out ItemData data);
        return data;
    }
}
