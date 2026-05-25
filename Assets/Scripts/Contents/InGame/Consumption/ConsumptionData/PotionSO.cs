using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ConsumptionData
{
    public int HpAmount { get; }

    public Potion(int hpAmount)
    {
        HpAmount = hpAmount;
    }

    public override void Use(GameObject user)
    {
        user.GetComponent<CharacterHealth>()?.RPC_AddHP(HpAmount);
    }
}

[CreateAssetMenu(fileName = "PotionSO", menuName = "Consumption/Potion")]
public class PotionSO : ConsumptionSO
{
    [SerializeField] private int hpAmount;

    public override void Load(ConsumptionManager manager)
        => manager.Register(consumptionId, new Potion(hpAmount));
}