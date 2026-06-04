using System.Collections.Generic;
using UnityEngine;

public class AttackConsumptionMapping : MonoBehaviour
{
    public static AttackConsumptionMapping Instance { get; private set; }

    [System.Serializable]
    private struct Entry
    {
        public bool enabled;
        public int attackId;
        public int consumptionId;
    }

    [SerializeField] private List<Entry> entries = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TryGetConsumption(int attackId, out int consumptionId)
    {
        foreach (var e in entries)
        {
            if (e.enabled && e.attackId == attackId)
            {
                consumptionId = e.consumptionId;
                return true;
            }
        }
        consumptionId = -1;
        return false;
    }

    public bool TryGetAttack(int consumptionId, out int attackId)
    {
        foreach (var e in entries)
        {
            if (e.enabled && e.consumptionId == consumptionId)
            {
                attackId = e.attackId;
                return true;
            }
        }
        attackId = -1;
        return false;
    }
}
