using Fusion;
using UnityEngine;

public abstract class AttackDataBase
{
    public NetworkObject Prefab;
    public int Damage;
    public float KnockbackForce;
    public Vector3 SpawnOffset;
    public int AttackVfxId;
    public int HitVfxId;
    public int CastSoundId;
    public int HitSoundId;
    public float HitStunDuration;
}

public abstract class AttackDataSO : ScriptableObject
{
    [SerializeField] private int id;
    public int Id => id;
    public abstract AttackDataBase GetData();
}

