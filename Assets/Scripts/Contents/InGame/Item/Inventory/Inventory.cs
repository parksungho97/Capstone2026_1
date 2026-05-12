using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int slotCapacity = 20;

    private struct SlotItemMapping
    {
        public Item item;
        public bool isUsed;
    }

    private SlotManager _slotManager;
    private SlotItemMapping[] _mappings;

    public event Action<int, Item, int> OnSlotUpdate; // slotIndex, item, count
    public event Action<int> OnSlotRemove; // slotIndex

    private void Start()
    {
        _slotManager = new SlotManager(slotCapacity);
        _mappings = new SlotItemMapping[slotCapacity];
    }

    public bool AddItem(Item item, int count = 1)
    {
        Debug.Assert(count > 0, "추가하려는 수량은 0보다 커야 합니다.");
        int remaining = count;

        // 1. 기존에 같은 아이템이 있는 슬롯에 먼저 채우기 (Stacking)
        for (int i = 0; i < _mappings.Length && remaining > 0; i++)
        {
            if (!_mappings[i].isUsed) continue;
            if (_mappings[i].item != item) continue;
            if (_slotManager.IsSlotFull(i)) continue;

            remaining = _slotManager.AddCount(i, remaining);
            OnSlotUpdate?.Invoke(i, item, _slotManager.GetCount(i));
        }

        // 2. 남은 수량이 있다면 빈 슬롯을 찾아 새로 할당 (New Slots)
        // 수량이 많아 여러 슬롯을 차지해야 할 수도 있으므로 while을 사용합니다.
        while (remaining > 0)
        {
            int newIndex = _slotManager.Add(item.MaxCount);

            // 더 이상 들어갈 슬롯이 없음
            if (newIndex == -1)
            {
                Debug.LogWarning($"Inventory: 슬롯 부족으로 {item.Name} {remaining}개를 추가하지 못했습니다.");
                return false;
            }

            _mappings[newIndex].item = item;
            _mappings[newIndex].isUsed = true;

            remaining = _slotManager.AddCount(newIndex, remaining);
            OnSlotUpdate?.Invoke(newIndex, item, _slotManager.GetCount(newIndex));
        }

        return true; // 모든 수량 추가 성공
    }

    public bool UseItem(Item item, int count = 1)
    {
        Debug.Assert(count > 0);

        // 총 수량 체크
        if (GetTotalCount(item) < count)
            return false;

        int remaining = count;

        for (int i = _mappings.Length - 1; i >= 0 && remaining > 0; i--)
        {
            if (!_mappings[i].isUsed) continue;
            if (_mappings[i].item != item) continue;

            remaining = _slotManager.RemoveCount(i, remaining);

            if (_slotManager.IsSlotEmpty(i))
            {
                _slotManager.RemoveSlot(i);
                _mappings[i] = default;
                OnSlotRemove?.Invoke(i);
            }
            else
            {
                OnSlotUpdate?.Invoke(i, item, _slotManager.GetCount(i));
            }
        }

        return true;
    }

    public Item GetItem(int slotIndex)
    {
        Debug.Assert(IsValidIndex(slotIndex), $"slotIndex {slotIndex} 없음");
        return _mappings[slotIndex].item;
    }

    public int GetCount(int slotIndex)
    {
        Debug.Assert(IsValidIndex(slotIndex), $"slotIndex {slotIndex} 없음");
        return _slotManager.GetCount(slotIndex);
    }

    public int GetTotalCount(Item item)
    {
        int total = 0;
        for (int i = 0; i < _mappings.Length; i++)
            if (_mappings[i].isUsed && _mappings[i].item == item)
                total += _slotManager.GetCount(i);
        return total;
    }

    private bool IsValidIndex(int index)
        => index >= 0 && index < _mappings.Length && _mappings[index].isUsed;
}