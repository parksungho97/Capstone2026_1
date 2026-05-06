using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseLoader : MonoBehaviour
{
    [SerializeField] private ItemUseSystem itemUseSystem;

    private void Start()
    {
        Debug.Assert(itemUseSystem);

        //Item item = new Item(0, "", null, "");

        //itemUseSystem.Register(item.Id, new HpItemUse(10.0f));

        //itemUseSystem.Use(gameObject, item);
    }
}
