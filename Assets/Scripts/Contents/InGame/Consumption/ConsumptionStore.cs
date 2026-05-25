using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsumptionStore : MonoBehaviour
{
    public event Action OnItemAdded;
    public event Action<int> OnItemRemoved;

    public bool IsFull { get; set; }

    private Dictionary<int, int> counts = new();

    public bool HasItem(int consumptionId) => counts.ContainsKey(consumptionId);

    public void Add(int consumptionId, int count)
    {
        if (counts.ContainsKey(consumptionId))
        {
            counts[consumptionId] += count;
        }
        else
        {
            if (IsFull) return;
            counts[consumptionId] = count;
            OnItemAdded?.Invoke();
        }
    }

    public bool Consume(int consumptionId, GameObject user)
    {
        if (!counts.TryGetValue(consumptionId, out int count) || count <= 0)
            return false;

        if (!ConsumptionManager.Instance.TryGet(consumptionId, out ConsumptionData data))
            return false;

        counts[consumptionId] = count - 1;
        if (counts[consumptionId] == 0)
        {
            counts.Remove(consumptionId);
            OnItemRemoved?.Invoke(consumptionId);
        }

        data.Use(user);
        return true;
    }

    public int GetCount(int consumptionId)
    {
        counts.TryGetValue(consumptionId, out int count);
        return count;
    }
}
