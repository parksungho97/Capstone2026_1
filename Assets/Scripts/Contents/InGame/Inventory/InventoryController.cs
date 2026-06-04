using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private int slotLimit = 20;

    public event Action<int, string, Sprite> OnEquipmentAdded;   // storeSlotIndex, name, icon
    public event Action<int> OnEquipmentRemoved;                   // storeSlotIndex
    public event Action<int, string, Sprite, int> OnConsumptionAdded;    // consumptionId, name, icon, count
    public event Action<int, string, Sprite, int> OnConsumptionUpdated;  // consumptionId, name, icon, count
    public event Action<int> OnConsumptionRemoved;                        // consumptionId

    private EquipmentStore equipmentStore;
    private ConsumptionStore consumptionStore;
    private int totalCount;
    private readonly Dictionary<int, (string Name, Sprite Icon)> consumptionItemData = new();

    private void Awake()
    {
        equipmentStore = GetComponent<EquipmentStore>();
        consumptionStore = GetComponent<ConsumptionStore>();

        equipmentStore.OnItemAdded += OnEquipmentItemAdded;
        equipmentStore.OnItemRemoved += OnEquipmentItemRemoved;
        consumptionStore.OnItemAdded += OnStoreItemAdded;
        consumptionStore.OnItemRemoved += OnConsumptionItemRemoved;
        consumptionStore.OnCountChanged += OnConsumptionCountChanged;
    }

    public bool AddItem(ItemId itemId, int count)
    {
        if (count <= 0) return false;
        if (!ItemMappings.Instance.TryGet(itemId, out EItemMapType type, out int typeValue))
            return false;

        ItemData itemData = ItemManager.Instance.Get(itemId);

        switch (type)
        {
            case EItemMapType.Equipment:
                if (EquipmentManager.Instance.TryGet(typeValue, out EquipmentData data))
                {
                    int slotIndex = equipmentStore.AddEquip(data.Generate());
                    return slotIndex >= 0;
                }
                return false;

            case EItemMapType.Consumption:
                bool isNew = !consumptionStore.HasItem(typeValue);
                if (!consumptionStore.Add(typeValue, count))
                    return false;
                if (itemData != null)
                {
                    consumptionItemData[typeValue] = (itemData.Name, itemData.Icon);
                    if (isNew)
                        OnConsumptionAdded?.Invoke(typeValue, itemData.Name, itemData.Icon, count);
                    else
                        OnConsumptionUpdated?.Invoke(typeValue, itemData.Name, itemData.Icon, consumptionStore.GetCount(typeValue));
                }
                return true;
        }

        return false;
    }

    private void OnEquipmentItemAdded(int slotIndex)
    {
        OnStoreItemAdded();

        Equipment equipment = equipmentStore.Get<Equipment>(slotIndex);
        if (equipment == null) return;

        if (!ItemMappings.Instance.TryGetItemByTypeValue(EItemMapType.Equipment, equipment.Id, out ItemId itemId))
            return;

        ItemData itemData = ItemManager.Instance.Get(itemId);
        if (itemData != null)
            OnEquipmentAdded?.Invoke(slotIndex, itemData.Name, itemData.Icon);
    }

    private void OnStoreItemAdded()
    {
        totalCount++;
        if (totalCount >= slotLimit)
            SetStoresFull(true);
    }

    private void OnStoreItemRemoved()
    {
        totalCount--;
        if (totalCount < slotLimit)
            SetStoresFull(false);
    }

    private void OnEquipmentItemRemoved(int storeSlotIndex)
    {
        OnStoreItemRemoved();
        OnEquipmentRemoved?.Invoke(storeSlotIndex);
    }

    private void OnConsumptionCountChanged(int consumptionId, int newCount)
    {
        if (!consumptionItemData.TryGetValue(consumptionId, out var data)) return;
        OnConsumptionUpdated?.Invoke(consumptionId, data.Name, data.Icon, newCount);
    }

    private void OnConsumptionItemRemoved(int consumptionId)
    {
        OnStoreItemRemoved();
        consumptionItemData.Remove(consumptionId);
        OnConsumptionRemoved?.Invoke(consumptionId);
    }

    private void SetStoresFull(bool isFull)
    {
        equipmentStore.IsFull = isFull;
        consumptionStore.IsFull = isFull;
    }
}
