using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : ConsumptionData
{
    public int ArmorAmount { get; }

    public Armor(int armorAmount)
    {
        ArmorAmount = armorAmount;
    }

    public override void Use(GameObject user) 
    { 
        user.GetComponent<CharacterHealth>()?.RPC_AddArmor(ArmorAmount);
    }
}

[CreateAssetMenu(fileName = "ArmorSO", menuName = "Consumption/Armor")]
public class ArmorSO : ConsumptionSO
{
    [SerializeField] private int armorAmount;

    public override void Load(ConsumptionManager manager)
        => manager.Register(consumptionId, new Armor(armorAmount));
}
