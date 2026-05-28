using Fusion;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    public int ItemIdValue { get; set; }
    public int Count { get; set; }

    public ItemId ItemId => new ItemId(ItemIdValue);

    public void SetItemData(ItemId id, int count)
    {
        ItemIdValue = id.Value;
        Count = count;
    }

    public void Take(InventoryController controller)
    {
        controller.AddItem(ItemId, Count);
        ItemInstanceManager.Instance.RequestDestroy(this);
    }
}
