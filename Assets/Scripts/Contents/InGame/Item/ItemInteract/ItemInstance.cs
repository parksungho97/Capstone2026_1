using Fusion;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] private int itemIdValue;
    [SerializeField] private int count;

    public int ItemIdValue { get => itemIdValue; set => itemIdValue = value; }
    public int Count { get => count; set => count = value; }

    public ItemId ItemId => new ItemId(ItemIdValue);

    public void SetItemData(ItemId id, int count)
    {
        ItemIdValue = id.Value;
        Count = count;
    }

    public void Take(InventoryController controller)
    {
        if (controller.AddItem(ItemId, Count))
            ItemInstanceManager.Instance.RequestDestroy(this);
    }
}
