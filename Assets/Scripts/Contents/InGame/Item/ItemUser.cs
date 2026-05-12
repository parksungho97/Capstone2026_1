using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUser : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemUseSystem itemUseSystem;

    private void Start()
    {
        Debug.Assert(inventory);
        Debug.Assert(itemUseSystem);
    }

    public void UseItem(Item item, int count = 1)
    {
        if (inventory.UseItem(item, count))
        {
            itemUseSystem.Use(item, gameObject);
            Debug.Log("sdf");
        }
    }
}
