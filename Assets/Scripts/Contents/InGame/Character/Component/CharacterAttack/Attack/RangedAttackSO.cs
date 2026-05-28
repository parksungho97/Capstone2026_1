using UnityEngine;


[System.Serializable]
public class RangedAttackData : AttackDataBase
{
    public float Speed;
    public float MaxDistance;
    public float Cooldown;
}


[CreateAssetMenu(fileName = "RangedAttackSO", menuName = "Attack/Ranged")]
public class RangedAttackSO : AttackDataSO
{
    [SerializeField] private RangedAttackData data;
    public override AttackDataBase GetData() => data;
}
