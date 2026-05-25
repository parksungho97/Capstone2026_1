using System;
using UnityEngine;

public interface IHotKeySlot
{
    string Name { get; }
    Sprite Icon { get; }
    void Execute();
}

public enum EHotKeyType : byte { K1, K2, K3, K4, Error }

public class HotKeyManager : MonoBehaviour
{
    public event Action<EHotKeyType, IHotKeySlot> OnSlotAssigned;
    public event Action<EHotKeyType> OnSlotCleared;
    public event Action<EHotKeyType> OnUIKeyClicked;

    private readonly IHotKeySlot[] slots = new IHotKeySlot[4];

    public void Assign(EHotKeyType key, IHotKeySlot slot)
    {
        if (key == EHotKeyType.Error) return;
        slots[(int)key] = slot;
        if (slot != null)
            OnSlotAssigned?.Invoke(key, slot);
        else
            OnSlotCleared?.Invoke(key);
    }

    public void Clear(EHotKeyType key)
    {
        Assign(key, null);
    }

    public void Swap(EHotKeyType keyA, EHotKeyType keyB)
    {
        if (keyA == EHotKeyType.Error || keyB == EHotKeyType.Error) return;

        IHotKeySlot temp = slots[(int)keyA];
        slots[(int)keyA] = slots[(int)keyB];
        slots[(int)keyB] = temp;

        NotifySlot(keyA);
        NotifySlot(keyB);
    }

    public IHotKeySlot Get(EHotKeyType key)
    {
        return key != EHotKeyType.Error ? slots[(int)key] : null;
    }

    public void NotifyUIClicked(EHotKeyType key)
    {
        OnUIKeyClicked?.Invoke(key);
    }

    private void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                slots[i]?.Execute();
        }
    }

    private void NotifySlot(EHotKeyType key)
    {
        IHotKeySlot slot = slots[(int)key];
        if (slot != null)
            OnSlotAssigned?.Invoke(key, slot);
        else
            OnSlotCleared?.Invoke(key);
    }
}
