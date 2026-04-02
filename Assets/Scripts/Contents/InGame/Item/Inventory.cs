using System;
using System.Collections.Generic;
using System.Diagnostics;

public class ItemSlot
{
    public ItemData Data { get; }
    public int Count { get; set; }

    public ItemSlot(ItemData data)
    {
        Data = data;
    }

    public void Add(int amount)
    {
        Count += amount;
    }

    public void Sub(int amount)
    {
        Count -= amount;
    }
}

public class Inventory
{
    private List<ItemSlot> slots;

    public IReadOnlyList<ItemSlot> Slots => slots;

    public event Action OnChanged;

    public void Regist(ItemData item, int amount)
    {
        Debug.Assert(item.maxCount <= amount, $"Inventory Regist: Item.MaxCount Check, {item.maxCount}, {amount}");

        slots.Add(new ItemSlot(item) { Count = amount });
    }

    public void Add(ItemData item, int count = 1)
    {
        ItemSlot slot = GetSlot(item.id);
        Debug.Assert(slot != null, $"Inventory Add: ItemSlot Check, {item.id}");

        slot.Add(count);

        if (slot.Count > item.maxCount)
        {
            int excess = slot.Count - item.maxCount;
            Regist(item, excess);

            slot.Count = item.maxCount;
        }

        OnChanged?.Invoke();
    }

    public void Remove(int itemId, int count = 1)
    {
        ItemSlot slot = GetSlot(itemId);
        Debug.Assert(slot != null, $"Inventory Remove: ItemSlot Check, {itemId}");

        slot.Sub(count);
        if (slot.Count <= 0)
            slots.Remove(slot);

        OnChanged?.Invoke();
    }

    private ItemSlot GetSlot(int itemId)
    {
        foreach (var slot in slots)
        {
            if (slot.Data.id == itemId)
                return slot;
        }
        return null;
    }
}