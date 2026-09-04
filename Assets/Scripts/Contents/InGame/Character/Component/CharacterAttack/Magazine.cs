using UnityEngine;

public class Magazine : MonoBehaviour
{
    public int CurrentProjectileId { get; private set; } = -1;

    private ConsumptionStore consumptionStore;
    private EquipmentSlot equipmentSlot;

    private void Start()
    {
        consumptionStore = GetComponent<ConsumptionStore>();
        equipmentSlot = GetComponent<EquipmentSlot>();
        Debug.Assert(consumptionStore != null);
        Debug.Assert(equipmentSlot != null);
    }

    public bool TryConsume()
    {
        if (!TryFindLoaded(out int attackId, out int consumptionId))
            return false;

        CurrentProjectileId = attackId;
        return consumptionStore.Consume(consumptionId, gameObject);
    }

    private bool TryFindLoaded(out int attackId, out int consumptionId)
    {
        Weapon weapon = equipmentSlot?.Weapon;
        if (weapon == null)
        {
            attackId = -1;
            consumptionId = -1;
            return false;
        }

        foreach (int id in weapon.AttackIds)
        {
            if (AttackConsumptionMapping.Instance.TryGetConsumption(id, out consumptionId)
                && consumptionStore.HasItem(consumptionId))
            {
                attackId = id;
                return true;
            }
        }

        attackId = -1;
        consumptionId = -1;
        return false;
    }
}
