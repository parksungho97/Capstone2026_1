using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

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

    public void RegistItem(int id, Item item)
    {
        if (_items.ContainsKey(id))
        {
            Debug.LogWarning($"ItemLoader: {id} 이미 등록됨");
            return;
        }
        _items[id] = item;
    }

    public Item Get(int id)
    {
        Debug.Assert(_items.ContainsKey(id), $"ItemLoader: {id} 아이템 없음");
        return _items[id];
    }

    private Dictionary<int, Item> _items = new();
}