using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseSystem : MonoBehaviour
{
    public void Register(Item item, ItemUse itemUse)
    {
        itemUseMappings[item] = itemUse;
    }

    public void Use(Item item, GameObject user)
    {
        if (itemUseMappings.TryGetValue(item, out var itemUse))
            itemUse.UseItem(user);
    }

    private Dictionary<Item, ItemUse> itemUseMappings = new();

    public static ItemUseSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}