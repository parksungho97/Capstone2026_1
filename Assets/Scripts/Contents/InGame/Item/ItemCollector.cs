using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    public void Initialize(ItemManager itemManager, Inventory inventory)
    {
        this.itemManager = itemManager;
        this.inventory = inventory;
    }

    public void AcquireItemOne()
    {
        if (itemManager == null)
        {
            Debug.Log("ItemCollector: ItemManager Is Null");
            return;
        }

        if (itemsInRange.Count == 0)
        {
            Debug.Log("주변에 획득 가능한 아이템이 없습니다.");
            return;
        }

        int randomIndex = Random.Range(0, itemsInRange.Count);
        ItemInstance targetInstance = System.Linq.Enumerable.ElementAt(itemsInRange, randomIndex);

        if (targetInstance == null) 
            return;

        Item itemData = itemManager.Get(targetInstance.ItemId);
        if (itemData == null) 
            return;

        bool isAdded = inventory.AddItem(itemData, 1);

        if (isAdded)
        {
            itemsInRange.Remove(targetInstance);
            Destroy(targetInstance.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        ItemInstance itemInstance = other.GetComponent<ItemInstance>();
        if (itemInstance)
            itemsInRange.Add(itemInstance);
    }

    public void OnTriggerExit(Collider other)
    {
        ItemInstance itemInstance = other.GetComponent<ItemInstance>();
        if (itemInstance)
            itemsInRange.Remove(itemInstance);
    }

    private HashSet<ItemInstance> itemsInRange = new HashSet<ItemInstance>();

    private ItemManager itemManager;
    private Inventory inventory;
}
