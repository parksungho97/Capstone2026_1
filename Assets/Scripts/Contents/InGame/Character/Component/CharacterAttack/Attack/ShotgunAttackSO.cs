using UnityEngine;

[System.Serializable]
public class ShotgunAttackData : AttackDataBase
{
    public float ActivationTime;
    public float Duration;
    public Vector3 InitialScale;
    public Vector3 FinalScale;
    public float DashSpeed;
}

[CreateAssetMenu(fileName = "ShotgunAttackSO", menuName = "Attack/Shotgun")]
public class ShotgunAttackSO : AttackDataSO
{
    [SerializeField] private ShotgunAttackData data;
    public override AttackDataBase GetData() => data;
}
