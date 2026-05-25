using Fusion;
using UnityEngine;

public abstract class AttackDataBase
{
    public NetworkObject Prefab;
    public int Damage;
    public float KnockbackForce;
    public Vector3 SpawnOffset;
}

public abstract class AttackDataSO : ScriptableObject
{
    [SerializeField] private int id;
    public int Id => id;
    public abstract AttackDataBase GetData();
}

