using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private InventoryUIMapper inventoryUIMapper;

    private void Start()
    {
        Debug.Assert(inventory, "Inventory가 연결되지 않았습니다.");
        Debug.Assert(inventoryUIMapper, "InventoryUIController가 연결되지 않았습니다.");

        ItemLoader itemLoader = ItemLoader.Instance;
        Debug.Assert(itemLoader != null);

        inventoryUIMapper.Initialize(inventoryUI, itemLoader);
        inventory.ActionItemAdd += inventoryUIMapper.OnItemAdd;
        inventory.ActionItemSub += inventoryUIMapper.OnItemSub;
        inventory.ActionItemRemove += inventoryUIMapper.OnItemRemove;
    }
}
