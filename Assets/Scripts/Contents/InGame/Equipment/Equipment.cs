using System.Collections.Generic;

public abstract class Equipment
{
}

public class Helmet : Equipment
{
    public int Armor { get; }

    public Helmet(int armor)
    {
        Armor = armor;
    }
}

public enum EAttackType
{
    Melee,
    Ranged,
}
public class Weapon : Equipment
{
    public EAttackType AttackType { get; }
    public IReadOnlyList<int> AttackIds { get; }
    public int AttackPower { get; }

    public Weapon(EAttackType attackType, IReadOnlyList<int> attackIds, int attackPower)
    {
        AttackType = attackType;
        AttackIds = new List<int>(attackIds);
        AttackPower = attackPower;
    }
}
