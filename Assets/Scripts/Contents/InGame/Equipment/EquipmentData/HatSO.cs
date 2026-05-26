using UnityEngine;

public class HatData : EquipmentData
{
    public int Armor { get; }

    public HatData(int id, int armor)
    {
        Id = id;
        Armor = armor;
    }

    public override Equipment Generate() => new Helmet(Id, Armor);
}

[CreateAssetMenu(fileName = "HatSO", menuName = "Equipment/Hat")]
public class HatSO : EquipmentSO
{
    [SerializeField] private int armor;

    public override void Load(EquipmentManager manager)
        => manager.Register(equipId, new HatData(equipId, armor));
}