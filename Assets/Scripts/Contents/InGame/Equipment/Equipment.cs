using System.Collections.Generic;

public abstract class Equipment
{
    public int Id { get; }

    protected Equipment(int id)
    {
        Id = id;
    }
}

public class Helmet : Equipment
{
    public int Armor { get; }

    public Helmet(int id, int armor) : base(id)
    {
        Armor = armor;
    }
}

public enum EAttackType
{
    Melee,
    Ranged,
    Shotgun,
}
public class Weapon : Equipment
{
    public EAttackType AttackType { get; }
    public IReadOnlyList<int> AttackIds { get; }
    public int AttackPower { get; }

    public Weapon(int id, EAttackType attackType, IReadOnlyList<int> attackIds, int attackPower) 
        : base(id)
    {
        AttackType = attackType;
        AttackIds = new List<int>(attackIds);
        AttackPower = attackPower;
    }
}
