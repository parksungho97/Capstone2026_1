using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    public int Count { get; set; }
    public int MaxCount { get; }

    public bool IsFull => Count >= MaxCount;
    public bool IsEmpty => Count <= 0;

    public ItemSlot(int count, int maxCount)
    {
        Count = count;
        MaxCount = maxCount;
    }
}

public class SlotManager
{
    private readonly int _capacity;
    private readonly Dictionary<int, ItemSlot> _slots = new Dictionary<int, ItemSlot>();
    private int _nextKey = 0;

    public int Capacity => _capacity;
    public int UsedCount => _slots.Count;
    public bool IsFull => _slots.Count >= _capacity;

    public IReadOnlyDictionary<int, ItemSlot> Slots => _slots;

    public SlotManager(int capacity)
    {
        _capacity = capacity;
    }

    public int Add(int count, int maxCount)
    {
        if (IsFull)
        {
            Debug.LogWarning("SlotManager: 사용 가능한 슬롯이 없습니다.");
            return -1;
        }

        int key = _nextKey++;
        _slots.Add(key, new ItemSlot(count, maxCount));
        return key;
    }

    public void Remove(int key)
    {
        if (!_slots.ContainsKey(key))
        {
            Debug.LogError($"SlotManager: key {key}에 해당하는 슬롯이 없습니다.");
            return;
        }
        _slots.Remove(key);
    }

    public ItemSlot Get(int key)
    {
        if (!_slots.TryGetValue(key, out ItemSlot slot))
        {
            Debug.LogError($"SlotManager: key {key}에 해당하는 슬롯이 없습니다.");
            return null;
        }
        return slot;
    }

    public bool TryGet(int key, out ItemSlot slot)
    {
        return _slots.TryGetValue(key, out slot);
    }
}
