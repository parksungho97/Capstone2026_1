using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int slotCapacity = 20;

    private SlotManager _slotManager;
    private readonly Dictionary<int, int> _itemIdToSlotKey = new Dictionary<int, int>();

    public SlotManager SlotManager => _slotManager;

    public event Action<int, int, int> ActionItemAdd;    // itemId, count, maxCount
    public event Action<int, int, int> ActionItemSub;    // itemId, count, maxCount
    public event Action<int> ActionItemRemove;           // itemId

    private void Awake()
    {
        _slotManager = new SlotManager(slotCapacity);
    }

    public void AddItem(int itemId, int count, int maxCount)
    {
        Debug.Assert(count > 0, "Inventory>> count must be greater than 0");
        Debug.Assert(count <= maxCount, "Inventory>> count must be <= maxCount");
        Debug.Assert(!_itemIdToSlotKey.ContainsKey(itemId), $"Inventory>> itemId {itemId}는 이미 슬롯이 존재합니다.");

        int slotKey = _slotManager.Add(count, maxCount);
        if (slotKey == -1) return;

        _itemIdToSlotKey[itemId] = slotKey;
        ActionItemAdd?.Invoke(itemId, count, maxCount);
    }

    public void Remove(int itemId, int count = 1)
    {
        if (!_itemIdToSlotKey.TryGetValue(itemId, out int slotKey))
        {
            Debug.LogError($"Inventory Remove: itemId {itemId}에 해당하는 슬롯이 없습니다.");
            return;
        }

        ItemSlot slot = _slotManager.Get(slotKey);
        slot.Count -= count;

        if (slot.Count <= 0)
        {
            _slotManager.Remove(slotKey);
            _itemIdToSlotKey.Remove(itemId);
            ActionItemRemove?.Invoke(itemId);
        }
        else
        {
            ActionItemSub?.Invoke(itemId, slot.Count, slot.MaxCount);
        }
    }

    public int GetTotalCount(int itemId)
    {
        if (!_itemIdToSlotKey.TryGetValue(itemId, out int slotKey))
            return 0;

        if (_slotManager.TryGet(slotKey, out ItemSlot slot))
            return slot.Count;

        return 0;
    }
}