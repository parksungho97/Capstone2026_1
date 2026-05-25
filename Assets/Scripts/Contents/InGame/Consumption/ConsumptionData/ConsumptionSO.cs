using UnityEngine;

public abstract class ConsumptionData
{
    public abstract void Use(GameObject user);
}

public abstract class ConsumptionSO : ScriptableObject
{
    [SerializeField] protected int consumptionId;

    public abstract void Load(ConsumptionManager manager);
}


