using System.Collections.Generic;
using UnityEngine;

public class ConsumptionStore : MonoBehaviour
{
    private Dictionary<int, int> counts = new();

    public void Add(int consumptionId, int count)
    {
        if (counts.ContainsKey(consumptionId))
            counts[consumptionId] += count;
        else
            counts[consumptionId] = count;
    }

    public bool Consume(int consumptionId, GameObject user)
    {
        if (!counts.TryGetValue(consumptionId, out int count) || count <= 0)
            return false;

        if (!ConsumptionManager.Instance.TryGet(consumptionId, out ConsumptionData data))
            return false;

        counts[consumptionId] = count - 1;
        if (counts[consumptionId] == 0)
            counts.Remove(consumptionId);

        data.Use(user);
        return true;
    }

    public int GetCount(int consumptionId)
    {
        counts.TryGetValue(consumptionId, out int count);
        return count;
    }
}
