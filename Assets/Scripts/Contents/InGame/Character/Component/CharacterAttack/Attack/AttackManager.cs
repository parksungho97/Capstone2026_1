using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public static AttackManager Instance { get; private set; }

    [SerializeField] private AttackDataSO[] attackDataSOs;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var so in attackDataSOs)
            _data[so.Id] = so.GetData();
    }

    public T Get<T>(int id) where T : AttackDataBase
    {
        if (_data.TryGetValue(id, out AttackDataBase data))
            return data as T;
        return null;
    }

    private readonly Dictionary<int, AttackDataBase> _data = new();
}
