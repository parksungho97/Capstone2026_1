using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentData
{
    public int Id { get; protected set; }
    public abstract Equipment Generate();
}


public abstract class EquipmentSO : ScriptableObject
{
    [SerializeField] protected int equipId;

    public abstract void Load(EquipmentManager manager);
}
