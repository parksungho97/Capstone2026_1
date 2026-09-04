using UnityEngine;

[System.Serializable]
public class ShotgunAttackData : AttackDataBase
{
    public float ActivationTime;
    public float Cooldown;
    public float Speed;
    public float MaxDistance;
    public int PelletCount;
    public float SpreadAngle;
    public LayerMask ObstacleLayer;
}

[CreateAssetMenu(fileName = "ShotgunAttackSO", menuName = "Attack/Shotgun")]
public class ShotgunAttackSO : AttackDataSO
{
    [SerializeField] private ShotgunAttackData data;
    public override AttackDataBase GetData() => data;
}
