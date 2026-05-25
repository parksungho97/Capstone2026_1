using UnityEngine;

public class Magazine : MonoBehaviour
{
    private int currentProjectileId = -1;
    private EquipmentSlot equipmentComponent;
    private ConsumptionStore consumptionStore;

    public int CurrentProjectileId => currentProjectileId;

    private void Start()
    {
        equipmentComponent = GetComponent<EquipmentSlot>();
        consumptionStore = GetComponent<ConsumptionStore>();
        Debug.Assert(equipmentComponent != null);
        Debug.Assert(consumptionStore != null);
    }

    public void SetAttackInstance(int attackInstance)
    {
        currentProjectileId = attackInstance;
    }

    public bool TryConsume()
    {
        if (equipmentComponent.Weapon == null)
            return false;

        if (currentProjectileId == -1)
            return false;

        if (AttackConsumptionMapping.Instance.TryGetConsumption(currentProjectileId, out int consumptionId))
            return consumptionStore.Consume(consumptionId, gameObject);

        return true;
    }
}
