using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoader : MonoBehaviour
{
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private ItemUseSystem itemUseSystem;

    private void Start()
    {
        Debug.Assert(itemManager);
        Debug.Assert(itemUseSystem);

        Item item0 = new Item("Hp", null, "");
        itemManager.RegistItem(0, item0);
        itemUseSystem.Register(item0, new HpItemUse(10.0f));
    }
}
