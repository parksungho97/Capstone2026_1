using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : EquipmentData
{
    public EAttackType AttackType { get; }
    public IReadOnlyList<int> AttackIds { get; }
    public int AttackPower { get; }

    public WeaponData(EAttackType attackType, IReadOnlyList<int> attackIds, int attackPower)
    {
        AttackType = attackType;
        AttackIds = new List<int>(attackIds);
        AttackPower = attackPower;
    }

    public override Equipment Generate() => new Weapon(AttackType, AttackIds, AttackPower);
}


[CreateAssetMenu(fileName = "WeaponSO", menuName = "Equipment/Weapon")]
public class WeaponSO : EquipmentSO
{
    [SerializeField] private EAttackType attackType;
    [SerializeField] private List<int> attackIds;
    [SerializeField] private int attackPower;

    public override void Load(EquipmentManager manager)
        => manager.Register(equipId, new WeaponData(attackType, attackIds, attackPower));
}
