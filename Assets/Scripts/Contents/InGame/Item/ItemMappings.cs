using System.Collections.Generic;
using UnityEngine;

public enum EItemMapType { Equipment, Consumption }

public class ItemMappings : MonoBehaviour
{
    public static ItemMappings Instance { get; private set; }

    private Dictionary<ItemId, (EItemMapType type, int typeValue)> map = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(ItemId id, EItemMapType type, int typeValue)
    {
        map[id] = (type, typeValue);
    }

    public bool TryGet(ItemId id, out EItemMapType type, out int typeValue)
    {
        if (map.TryGetValue(id, out var e))
        {
            type = e.type;
            typeValue = e.typeValue;
            return true;
        }
        type = default;
        typeValue = -1;
        return false;
    }

    public bool TryGetItemByTypeValue(EItemMapType type, int typeValue, out ItemId itemId)
    {
        foreach (var kvp in map)
        {
            if (kvp.Value.type == type && kvp.Value.typeValue == typeValue)
            {
                itemId = kvp.Key;
                return true;
            }
        }
        itemId = default;
        return false;
    }
}
