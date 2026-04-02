using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealData : ItemUseData
{
    public HealData(int id, string itemName, string description, Sprite icon, int maxCount, int healAmount)
        : base(id, itemName, description, icon, maxCount, EItemUseType.Heal)
    {
        HealAmount = healAmount;
    }
    public int HealAmount { get; private set; }
}

public class HealEffect : ItemUsable
{
    public HealEffect()
        : base(EItemUseType.Heal)
    { }
    public void Heal(HealData healData)
    {
        Debug.Log($"HealAmount: {healData.HealAmount}");
    }
}

[CreateAssetMenu(menuName = "Item/HealData")]
public class HealDataSO : ItemUseSO
{
    [SerializeField] private int healAmount;
    public int HealAmount => healAmount;

    public override ItemData ToItemData()
    {
        return new HealData(id, itemName, description, icon, maxCount, HealAmount);
    }
}