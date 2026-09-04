using UnityEngine;

public abstract class ConsumptionData
{
    public int MaxCount { get; protected set; }
    public abstract void Use(GameObject user);
}

public abstract class ConsumptionSO : ScriptableObject
{
    [SerializeField] protected int consumptionId;
    [SerializeField] protected int maxCount;

    public abstract void Load(ConsumptionManager manager);
}


