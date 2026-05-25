using System.Collections.Generic;
using UnityEngine;

public class ConsumptionManager : MonoBehaviour
{
    public static ConsumptionManager Instance { get; private set; }

    [SerializeField] private List<ConsumptionSO> consumptionSOs;

    private Dictionary<int, ConsumptionData> map = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var so in consumptionSOs)
            so.Load(this);
    }

    public void Register(int id, ConsumptionData data)
    {
        map[id] = data;
    }

    public bool TryGet(int id, out ConsumptionData data)
        => map.TryGetValue(id, out data);
}
