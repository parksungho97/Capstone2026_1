//using System;
//using UnityEngine;

//public class Inventory : MonoBehaviour
//{
//    [SerializeField] private int slotCapacity = 20;

//    private struct SlotItemMapping
//    {
//        public ItemId itemId;
//        public bool isUsed;
//    }

//    private SlotManager _slotManager;
//    private SlotItemMapping[] _mappings;

//    public event Action<int, ItemId, int> OnSlotUpdate; // slotIndex, itemId, count
//    public event Action<int, ItemId> OnSlotRemove;       // slotIndex, itemId

//    private void Start()
//    {
//        _slotManager = new SlotManager(slotCapacity);
//        _mappings = new SlotItemMapping[slotCapacity];
//    }

//    public bool AddItem(ItemId id, int count, int maxCount)
//    {
//        Debug.Assert(count > 0, "추가하려는 수량은 0보다 커야 합니다.");
//        int remaining = count;

//        for (int i = 0; i < _mappings.Length && remaining > 0; i++)
//        {
//            if (!_mappings[i].isUsed) continue;
//            if (_mappings[i].itemId != id) continue;
//            if (_slotManager.IsSlotFull(i)) continue;

//            remaining = _slotManager.AddCount(i, remaining);
//            OnSlotUpdate?.Invoke(i, id, _slotManager.GetCount(i));
//        }

//        while (remaining > 0)
//        {
//            int newIndex = _slotManager.Add(maxCount);

//            if (newIndex == -1)
//            {
//                Debug.LogWarning($"Inventory: 슬롯 부족으로 {id} {remaining}개를 추가하지 못했습니다.");
//                return false;
//            }

//            _mappings[newIndex].itemId = id;
//            _mappings[newIndex].isUsed = true;

//            remaining = _slotManager.AddCount(newIndex, remaining);
//            OnSlotUpdate?.Invoke(newIndex, id, _slotManager.GetCount(newIndex));
//        }

//        return true;
//    }

//    public bool UseItem(ItemId id, int count = 1)
//    {
//        Debug.Assert(count > 0);

//        if (GetTotalCount(id) < count)
//            return false;

//        int remaining = count;

//        for (int i = _mappings.Length - 1; i >= 0 && remaining > 0; i--)
//        {
//            if (!_mappings[i].isUsed) continue;
//            if (_mappings[i].itemId != id) continue;

//            remaining = _slotManager.RemoveCount(i, remaining);

//            if (_slotManager.IsSlotEmpty(i))
//            {
//                _slotManager.RemoveSlot(i);
//                _mappings[i] = default;
//                OnSlotRemove?.Invoke(i, id);
//            }
//            else
//            {
//                OnSlotUpdate?.Invoke(i, id, _slotManager.GetCount(i));
//            }
//        }

//        return true;
//    }

//    public ItemId GetItem(int slotIndex)
//    {
//        Debug.Assert(IsValidIndex(slotIndex), $"slotIndex {slotIndex} 없음");
//        return _mappings[slotIndex].itemId;
//    }

//    public int GetCount(int slotIndex)
//    {
//        Debug.Assert(IsValidIndex(slotIndex), $"slotIndex {slotIndex} 없음");
//        return _slotManager.GetCount(slotIndex);
//    }

//    public int GetTotalCount(ItemId id)
//    {
//        int total = 0;
//        for (int i = 0; i < _mappings.Length; i++)
//            if (_mappings[i].isUsed && _mappings[i].itemId == id)
//                total += _slotManager.GetCount(i);
//        return total;
//    }

//    private bool IsValidIndex(int index)
//        => index >= 0 && index < _mappings.Length && _mappings[index].isUsed;
//}
