using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ConsumptionData
{
    public int AttackPower { get; }

    public Bullet(int attackPower, int maxCount)
    {
        AttackPower = attackPower;
        MaxCount = maxCount;
    }

    public override void Use(GameObject user) { }
}


[CreateAssetMenu(fileName = "BulletSO", menuName = "Consumption/Bullet")]
public class BulletSO : ConsumptionSO
{
    [SerializeField] private int attackPower;

    public override void Load(ConsumptionManager manager)
        => manager.Register(consumptionId, new Bullet(attackPower, maxCount));
}
