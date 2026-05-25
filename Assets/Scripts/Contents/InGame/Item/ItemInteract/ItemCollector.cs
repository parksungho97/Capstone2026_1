using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private InventoryController inventoryController;
    private ItemInstance inst;

    private void Start()
    {
        Debug.Assert(inventoryController);
    }
    public void AcquireOne()
    {
        if (inst == null) return;
        inst.Take(inventoryController);
        inst = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemInstance item = other.GetComponent<ItemInstance>();
        if (item != null) inst = item;
    }

    private void OnTriggerExit(Collider other)
    {
        ItemInstance item = other.GetComponent<ItemInstance>();
        if (item == inst) inst = null;
    }
}
