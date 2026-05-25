using System;
using UnityEngine;

public enum EquipmentType { Helmet, Weapon }

public class EquipmentStore : MonoBehaviour
{
    public event Action<int> OnItemAdded;   // slot index
    public event Action<int> OnItemRemoved; // slot index

    public bool IsFull { get; set; }

    private const int Capacity = 10;
    private readonly Equipment[] slots = new Equipment[Capacity];

    public int AddEquip(Equipment equipment)
    {
        if (IsFull) return -1;

        for (int i = 0; i < Capacity; i++)
        {
            if (slots[i] != null) continue;
            slots[i] = equipment;
            OnItemAdded?.Invoke(i);
            return i;
        }
        return -1;
    }

    public T Get<T>(int index) where T : Equipment
    {
        if (index < 0 || index >= Capacity) return null;
        return slots[index] as T;
    }

    public bool Remove(int index)
    {
        if (index < 0 || index >= Capacity) return false;
        if (slots[index] == null) return false;
        slots[index] = null;
        OnItemRemoved?.Invoke(index);
        return true;
    }

    public bool Remove(Equipment equipment)
    {
        for (int i = 0; i < Capacity; i++)
        {
            if (slots[i] != equipment) continue;
            slots[i] = null;
            OnItemRemoved?.Invoke(i);
            return true;
        }
        return false;
    }
}
