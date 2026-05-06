using UnityEngine;

public class Slot
{
    public int Count { get; private set; }
    public int MaxCount { get; }

    public bool IsFull => Count >= MaxCount;
    public bool IsEmpty => Count <= 0;

    public Slot(int maxCount)
    {
        MaxCount = maxCount;
        Count = 0;
    }

    public int Add(int count)
    {
        int overflow = Mathf.Max(0, Count + count - MaxCount);
        Count = Mathf.Min(Count + count, MaxCount);
        return overflow;
    }

    public int Remove(int count)
    {
        int lack = Mathf.Max(0, count - Count);
        Count = Mathf.Max(0, Count - count);
        return lack;
    }
}

public class SlotManager
{
    public int Capacity => _slots.Length;
    public int UsedCount => _usedCount;
    public bool IsFull => _usedCount >= _slots.Length;

    public SlotManager(int capacity)
    {
        _slots = new Slot[capacity];
    }

    // 새 슬롯 할당 → index 반환
    public int Add(int maxCount)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] != null) continue;
            _slots[i] = new Slot(maxCount);
            _usedCount++;
            return i;
        }
        Debug.LogWarning("SlotManager: 슬롯이 가득 찼습니다.");
        return -1;
    }

    public void RemoveSlot(int index)
    {
        Debug.Assert(IsValidIndex(index), $"SlotManager: index {index} 없음");
        _slots[index] = null;
        _usedCount--;
    }

    // 수량 추가 → overflow 반환
    public int AddCount(int index, int count)
    {
        Debug.Assert(IsValidIndex(index), $"SlotManager: index {index} 없음");
        return _slots[index].Add(count);
    }

    // 수량 제거 → lack 반환
    public int RemoveCount(int index, int count)
    {
        Debug.Assert(IsValidIndex(index), $"SlotManager: index {index} 없음");
        return _slots[index].Remove(count);
    }

    public int GetCount(int index)
    {
        Debug.Assert(IsValidIndex(index), $"SlotManager: index {index} 없음");
        return _slots[index].Count;
    }

    public int GetMaxCount(int index)
    {
        Debug.Assert(IsValidIndex(index), $"SlotManager: index {index} 없음");
        return _slots[index].MaxCount;
    }

    public bool IsSlotFull(int index)
    {
        Debug.Assert(IsValidIndex(index), $"SlotManager: index {index} 없음");
        return _slots[index].IsFull;
    }

    public bool IsSlotEmpty(int index)
    {
        Debug.Assert(IsValidIndex(index), $"SlotManager: index {index} 없음");
        return _slots[index].IsEmpty;
    }

    public bool IsValidIndex(int index)
        => index >= 0 && index < _slots.Length && _slots[index] != null;

    private readonly Slot[] _slots;
    private int _usedCount;
}