using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class HotKeySlot
{
    public abstract void Execute();
    public Sprite Icon { get; protected set; }
    public string Name { get; protected set; }
}

public enum EHotKeyType : byte
{
    K1, K2, K3, K4
}

public class HotKey : MonoBehaviour
{
    [SerializeField] private HotKeyUI hotKeyUI;

    public Action<EHotKeyType> ActionHotKeyClick;

    private void Awake()
    {
        Debug.Assert(hotKeyUI);

        KeyCode[] keyCodes = new KeyCode[4] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };
        hotKeyUI.Initialize(4, keyCodes);

        hotKeyUI.ActionHotKeyClicked += (KeyCode keyCode) =>
        {
            EHotKeyType type = (EHotKeyType)(keyCode - KeyCode.Alpha1);
            ActionHotKeyClick?.Invoke(type);
        };
    }
    public void SetSlot(EHotKeyType keyType, HotKeySlot slot)
    {
        hotKeySlots[(int)keyType] = slot;

        if (slot == null)
            hotKeyUI.RemoveHotKeyUI(KeyCode.Alpha1 + (int)keyType);
        else
            hotKeyUI.SetDescription(KeyCode.Alpha1 + (int)keyType, slot.Name, slot.Icon);
    }

    private void Update()
    {
        for (KeyCode keyCode = KeyCode.Alpha1; keyCode < KeyCode.Alpha4; ++keyCode)
        {
            if (Input.GetKeyDown(keyCode))
                hotKeySlots[keyCode - KeyCode.Alpha1].Execute();
        }
    }

    private HotKeySlot[] hotKeySlots = new HotKeySlot[4];
}