using UnityEngine;

public enum EquipmentType { Helmet, Weapon }

public class EquipmentStore : MonoBehaviour
{
    private const int Capacity = 10;
    private readonly Equipment[] slots = new Equipment[Capacity];

    public bool AddEquip(Equipment equipment)
    {
        for (int i = 0; i < Capacity; i++)
        {
            if (slots[i] != null) continue;
            slots[i] = equipment;
            return true;
        }
        return false;
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
        return true;
    }

    public bool Remove(Equipment equipment)
    {
        for (int i = 0; i < Capacity; i++)
        {
            if (slots[i] != equipment) continue;
            slots[i] = null;
            return true;
        }
        return false;
    }
}
