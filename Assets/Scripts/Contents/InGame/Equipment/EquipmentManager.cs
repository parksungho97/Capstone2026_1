using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    [SerializeField] private List<EquipmentSO> equipmentSOs;

    private Dictionary<int, EquipmentData> map = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var so in equipmentSOs)
            so.Load(this);
    }

    public void Register(int id, EquipmentData data)
    {
        map[id] = data;
    }

    public bool TryGet(int equipId, out EquipmentData data)
        => map.TryGetValue(equipId, out data);
}
