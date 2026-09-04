using UnityEngine;

[System.Serializable]
public class MeleeAttackData : AttackDataBase
{
    public float ActivationTime;
    public float Duration;
}


[CreateAssetMenu(fileName = "MeleeAttackSO", menuName = "Attack/Melee")]
public class MeleeAttackSO : AttackDataSO
{
    [SerializeField] private MeleeAttackData data;
    public override AttackDataBase GetData() => data;
}
