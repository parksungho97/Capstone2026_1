using UnityEngine;

public class HotKeyUIMapper : MonoBehaviour
{
    [SerializeField] private HotKeyManager hotKeyManager;
    [SerializeField] private HotKeyUI hotKeyUI;

    private void Awake()
    {
        KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        hotKeyUI.Initialize(4, keyCodes);

        hotKeyManager.OnSlotAssigned += OnSlotAssigned;
        hotKeyManager.OnSlotCleared += OnSlotCleared;
        hotKeyUI.ActionHotKeyClicked += OnUIKeyClicked;
    }

    private void OnSlotAssigned(EHotKeyType key, IHotKeySlot slot)
    {
        hotKeyUI.SetDescription(KeyCode.Alpha1 + (int)key, slot.Name, slot.Icon);
    }

    private void OnSlotCleared(EHotKeyType key)
    {
        hotKeyUI.RemoveHotKeyUI(KeyCode.Alpha1 + (int)key);
    }

    private void OnUIKeyClicked(KeyCode keyCode)
    {
        EHotKeyType type = (EHotKeyType)(keyCode - KeyCode.Alpha1);
        hotKeyManager.NotifyUIClicked(type);
    }
}
