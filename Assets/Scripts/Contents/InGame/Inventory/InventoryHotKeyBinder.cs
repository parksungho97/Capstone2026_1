using System.Collections.Generic;
using UnityEngine;

public class InventoryHotKeyBinder : MonoBehaviour
{
    [SerializeField] private InventoryUIMapper inventoryUIMapper;
    [SerializeField] private HotKeyManager hotKeyManager;

    private int pendingUiSlotIndex = -1;
    private ConsumptionStore consumptionStore;

    private readonly Dictionary<int, EHotKeyType> consumptionToHotKey = new();
    private readonly Dictionary<EHotKeyType, int> hotKeyToConsumption = new();

    private void Awake()
    {
        InventoryUI ui = inventoryUIMapper.InventoryUI;
        ui.ActionSlotClicked += OnInventorySlotClicked;
        ui.ActionDisabled += OnInventoryDisabled;

        hotKeyManager.OnUIKeyClicked += OnHotKeySlotClicked;
        inventoryUIMapper.OnControllerLinked += OnControllerLinked;
    }

    private void OnControllerLinked()
    {
        if (consumptionStore != null)
            consumptionStore.OnItemRemoved -= OnConsumptionRemoved;

        foreach (EHotKeyType key in hotKeyToConsumption.Keys)
            hotKeyManager.Clear(key);

        consumptionToHotKey.Clear();
        hotKeyToConsumption.Clear();
        pendingUiSlotIndex = -1;

        consumptionStore = inventoryUIMapper.ConsumptionStore;

        if (consumptionStore != null)
            consumptionStore.OnItemRemoved += OnConsumptionRemoved;
    }

    private void OnInventorySlotClicked(int uiSlotIndex)
    {
        if (!inventoryUIMapper.TryGetConsumptionByUiSlot(uiSlotIndex, out int consumptionId, out _, out _, out _))
        {
            pendingUiSlotIndex = -1;
            return;
        }

        if (ConsumptionManager.Instance.TryGet(consumptionId, out ConsumptionData data) && data is Bullet)
        {
            pendingUiSlotIndex = -1;
            return;
        }

        pendingUiSlotIndex = uiSlotIndex;
    }

    private void OnInventoryDisabled()
    {
        pendingUiSlotIndex = -1;
    }

    private void OnHotKeySlotClicked(EHotKeyType key)
    {
        if (pendingUiSlotIndex < 0) return;

        if (!inventoryUIMapper.TryGetConsumptionByUiSlot(pendingUiSlotIndex, out int consumptionId, out string name, out Sprite icon, out _))
        {
            pendingUiSlotIndex = -1;
            return;
        }

        if (hotKeyToConsumption.TryGetValue(key, out int previousId) && previousId != consumptionId)
        {
            consumptionToHotKey.Remove(previousId);
            hotKeyToConsumption.Remove(key);
        }

        if (consumptionToHotKey.TryGetValue(consumptionId, out EHotKeyType previousKey) && previousKey != key)
        {
            hotKeyToConsumption.Remove(previousKey);
            hotKeyManager.Clear(previousKey);
        }

        consumptionToHotKey[consumptionId] = key;
        hotKeyToConsumption[key] = consumptionId;

        hotKeyManager.Assign(key, new ConsumptionHotKeySlot(name, icon, consumptionId, consumptionStore, inventoryUIMapper.UserGameObject));
        pendingUiSlotIndex = -1;
    }

    private void OnConsumptionRemoved(int consumptionId)
    {
        if (!consumptionToHotKey.TryGetValue(consumptionId, out EHotKeyType key)) return;

        consumptionToHotKey.Remove(consumptionId);
        hotKeyToConsumption.Remove(key);
        hotKeyManager.Clear(key);
    }

    private class ConsumptionHotKeySlot : IHotKeySlot
    {
        public string Name { get; }
        public Sprite Icon { get; }

        private readonly int consumptionId;
        private readonly ConsumptionStore store;
        private readonly GameObject user;

        public ConsumptionHotKeySlot(string name, Sprite icon, int consumptionId, ConsumptionStore store, GameObject user)
        {
            Name = name;
            Icon = icon;
            this.consumptionId = consumptionId;
            this.store = store;
            this.user = user;
        }

        public void Execute() => store.Consume(consumptionId, user);
    }
}
