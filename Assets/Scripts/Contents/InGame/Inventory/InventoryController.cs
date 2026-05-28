using System;
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

    private void Awake()
    {
        equipmentStore = GetComponent<EquipmentStore>();
        consumptionStore = GetComponent<ConsumptionStore>();

        equipmentStore.OnItemAdded += _ => OnStoreItemAdded();
        equipmentStore.OnItemRemoved += OnEquipmentItemRemoved;
        consumptionStore.OnItemAdded += OnStoreItemAdded;
        consumptionStore.OnItemRemoved += OnConsumptionItemRemoved;
    }

    public void AddItem(ItemId itemId, int count)
    {
        if (!ItemMappings.Instance.TryGet(itemId, out EItemMapType type, out int typeValue))
            return;

        ItemData itemData = ItemManager.Instance.Get(itemId);

        switch (type)
        {
            case EItemMapType.Equipment:
                if (EquipmentManager.Instance.TryGet(typeValue, out EquipmentData data))
                {
                    int slotIndex = equipmentStore.AddEquip(data.Generate());
                    if (slotIndex >= 0 && itemData != null)
                        OnEquipmentAdded?.Invoke(slotIndex, itemData.Name, itemData.Icon);
                }
                break;

            case EItemMapType.Consumption:
                bool isNew = !consumptionStore.HasItem(typeValue);
                consumptionStore.Add(typeValue, count);
                if (itemData == null) break;
                if (isNew && consumptionStore.HasItem(typeValue))
                    OnConsumptionAdded?.Invoke(typeValue, itemData.Name, itemData.Icon, count);
                else if (!isNew)
                    OnConsumptionUpdated?.Invoke(typeValue, itemData.Name, itemData.Icon, consumptionStore.GetCount(typeValue));
                break;
        }
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

    private void OnConsumptionItemRemoved(int consumptionId)
    {
        OnStoreItemRemoved();
        OnConsumptionRemoved?.Invoke(consumptionId);
    }

    private void SetStoresFull(bool isFull)
    {
        equipmentStore.IsFull = isFull;
        consumptionStore.IsFull = isFull;
    }
}
