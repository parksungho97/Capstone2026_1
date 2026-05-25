using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private EquipmentStore equipmentStore;
    private ConsumptionStore consumptionStore;

    private void Awake()
    {
        equipmentStore = GetComponent<EquipmentStore>();
        consumptionStore = GetComponent<ConsumptionStore>();
    }

    public void AddItem(ItemId itemId, int count)
    {
        if (!ItemMappings.Instance.TryGet(itemId, out EItemMapType type, out int typeValue))
            return;

        switch (type)
        {
            case EItemMapType.Equipment:
                if (EquipmentManager.Instance.TryGet(typeValue, out EquipmentData data))
                    equipmentStore.AddEquip(data.Generate());
                break;
            case EItemMapType.Consumption:
                consumptionStore.Add(typeValue, count);
                break;
        }
    }
}
