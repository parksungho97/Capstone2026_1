using System.Collections.Generic;
using UnityEngine;

public class AttackConsumptionMapping : MonoBehaviour
{
    public static AttackConsumptionMapping Instance { get; private set; }

    [System.Serializable]
    private struct Entry
    {
        public int attackId;
        public int consumptionId;
    }

    [SerializeField] private List<Entry> entries = new();

    private Dictionary<int, int> attackToConsumption = new();
    private Dictionary<int, int> consumptionToAttack = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var e in entries)
        {
            attackToConsumption[e.attackId] = e.consumptionId;
            consumptionToAttack[e.consumptionId] = e.attackId;
        }
    }

    public bool TryGetConsumption(int attackId, out int consumptionId)
        => attackToConsumption.TryGetValue(attackId, out consumptionId);

    public bool TryGetAttack(int consumptionId, out int attackId)
        => consumptionToAttack.TryGetValue(consumptionId, out attackId);
}
