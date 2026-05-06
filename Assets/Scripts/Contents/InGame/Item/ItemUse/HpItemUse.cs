using System.Collections.Generic;
using UnityEngine;

public abstract class ItemUse
{
    public abstract void UseItem(GameObject user);
}

public class HpItemUse : ItemUse
{
    public HpItemUse(float healAmount)
    {
        this.healAmount = healAmount;
    }
    public override void UseItem(GameObject user)
    {
        Debug.Log($"{healAmount}");
    }

    private float healAmount;
}