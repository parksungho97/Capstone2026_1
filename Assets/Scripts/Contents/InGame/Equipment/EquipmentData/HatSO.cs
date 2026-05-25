using UnityEngine;

public class HatData : EquipmentData
{
    public int Armor { get; }

    public HatData(int armor)
    {
        Armor = armor;
    }

    public override Equipment Generate() => new Helmet(Armor);
}

[CreateAssetMenu(fileName = "HatSO", menuName = "Equipment/Hat")]
public class HatSO : EquipmentSO
{
    [SerializeField] private int armor;

    public override void Load(EquipmentManager manager)
        => manager.Register(equipId, new HatData(armor));
}