using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIMapper : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;

    public event Action OnControllerLinked;

    public InventoryUI InventoryUI => inventoryUI;
    public ConsumptionStore ConsumptionStore => inventoryController != null ? inventoryController.GetComponent<ConsumptionStore>() : null;
    public GameObject UserGameObject => inventoryController != null ? inventoryController.gameObject : null;

    private void Awake()
    {
        inventoryUI.OnBecameVisible += Repopulate;
    }

    public void LinkInventoryController(InventoryController inventoryController)
    {
        if (this.inventoryController != null)
        {
            this.inventoryController.OnEquipmentAdded -= OnEquipmentAdded;
            this.inventoryController.OnEquipmentRemoved -= OnEquipmentRemoved;
            this.inventoryController.OnConsumptionAdded -= OnConsumptionAdded;
            this.inventoryController.OnConsumptionUpdated -= OnConsumptionUpdated;
            this.inventoryController.OnConsumptionRemoved -= OnConsumptionRemoved;
        }

        this.inventoryController = inventoryController;

        this.inventoryController.OnEquipmentAdded += OnEquipmentAdded;
        this.inventoryController.OnEquipmentRemoved += OnEquipmentRemoved;
        this.inventoryController.OnConsumptionAdded += OnConsumptionAdded;
        this.inventoryController.OnConsumptionUpdated += OnConsumptionUpdated;
        this.inventoryController.OnConsumptionRemoved += OnConsumptionRemoved;

        OnControllerLinked?.Invoke();
    }

    public bool TryGetEquipmentByUiSlot(int uiSlotIndex, out int storeSlotIndex)
    {
        foreach (var kvp in equipmentUiSlots)
        {
            if (kvp.Value != uiSlotIndex) continue;
            storeSlotIndex = kvp.Key;
            return true;
        }

        storeSlotIndex = -1;
        return false;
    }

    public bool TryGetConsumptionByUiSlot(int uiSlotIndex, out int consumptionId, out string name, out Sprite icon, out int count)
    {
        foreach (var kvp in consumptionUiSlots)
        {
            if (kvp.Value != uiSlotIndex) continue;

            consumptionId = kvp.Key;
            if (consumptionData.TryGetValue(consumptionId, out var data))
            {
                name = data.Name;
                icon = data.Icon;
                count = data.Count;
                return true;
            }
        }

        consumptionId = -1;
        name = null;
        icon = null;
        count = 0;
        return false;
    }

    private void OnEquipmentAdded(int storeSlotIndex, string name, Sprite icon)
    {
        equipmentData[storeSlotIndex] = (name, icon);

        int uiIndex = inventoryUI.AddItemUI(name, icon, 1);
        if (uiIndex >= 0)
            equipmentUiSlots[storeSlotIndex] = uiIndex;
    }

    private void OnEquipmentRemoved(int storeSlotIndex)
    {
        equipmentData.Remove(storeSlotIndex);

        if (equipmentUiSlots.TryGetValue(storeSlotIndex, out int uiIndex))
        {
            inventoryUI.RemoveItem(uiIndex);
            equipmentUiSlots.Remove(storeSlotIndex);
        }
    }

    private void OnConsumptionAdded(int consumptionId, string name, Sprite icon, int count)
    {
        consumptionData[consumptionId] = (name, icon, count);

        int uiIndex = inventoryUI.AddItemUI(name, icon, count);
        if (uiIndex >= 0)
            consumptionUiSlots[consumptionId] = uiIndex;
    }

    private void OnConsumptionUpdated(int consumptionId, string name, Sprite icon, int count)
    {
        consumptionData[consumptionId] = (name, icon, count);

        if (consumptionUiSlots.TryGetValue(consumptionId, out int uiIndex))
            inventoryUI.UpdateItemUI(uiIndex, name, icon, count);
    }

    private void OnConsumptionRemoved(int consumptionId)
    {
        consumptionData.Remove(consumptionId);

        if (consumptionUiSlots.TryGetValue(consumptionId, out int uiIndex))
        {
            inventoryUI.RemoveItem(uiIndex);
            consumptionUiSlots.Remove(consumptionId);
        }
    }

    private void Repopulate()
    {
        foreach (int uiIndex in equipmentUiSlots.Values)
            inventoryUI.RemoveItem(uiIndex);
        foreach (int uiIndex in consumptionUiSlots.Values)
            inventoryUI.RemoveItem(uiIndex);

        equipmentUiSlots.Clear();
        consumptionUiSlots.Clear();

        foreach (var kvp in equipmentData)
        {
            int uiIndex = inventoryUI.AddItemUI(kvp.Value.Name, kvp.Value.Icon, 1);
            if (uiIndex >= 0)
                equipmentUiSlots[kvp.Key] = uiIndex;
        }

        foreach (var kvp in consumptionData)
        {
            int uiIndex = inventoryUI.AddItemUI(kvp.Value.Name, kvp.Value.Icon, kvp.Value.Count);
            if (uiIndex >= 0)
                consumptionUiSlots[kvp.Key] = uiIndex;
        }
    }

    private readonly Dictionary<int, (string Name, Sprite Icon)> equipmentData = new();
    private readonly Dictionary<int, (string Name, Sprite Icon, int Count)> consumptionData = new();

    private readonly Dictionary<int, int> equipmentUiSlots = new();   // storeSlotIndex → uiSlotIndex
    private readonly Dictionary<int, int> consumptionUiSlots = new(); // consumptionId   → uiSlotIndex

    private InventoryController inventoryController;
}
