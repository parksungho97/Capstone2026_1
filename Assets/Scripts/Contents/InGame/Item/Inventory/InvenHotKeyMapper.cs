//using System.Collections.Generic;
//using UnityEngine;

//public class ItemHotKey : HotKeySlot
//{
//    public ItemHotKey(ItemId itemId, string name, Sprite icon, CharacterInventory characterInventory)
//    {
//        Name = name;
//        Icon = icon;
//        this.itemId = itemId;
//        this.characterInventory = characterInventory;
//    }

//    public override void Execute()
//    {
//        characterInventory.UseItem(itemId);
//    }

//    private ItemId itemId;
//    private CharacterInventory characterInventory;
//}

//public class InvenHotKeyMapper : MonoBehaviour
//{
//    [SerializeField] private InventoryUIMapper inventoryUIMapper;
//    [SerializeField] private HotKey hotKey;

//    public CharacterInventory CharacterInven { get; private set; }

//    public void Initalize(CharacterInventory characterInventory)
//    {
//        CharacterInven = characterInventory;

//        inventoryUIMapper.LinkedInventoryUI.ActionDisabled += Clear;

//        inventoryUIMapper.LinkedInventoryUI.ActionSlotClicked += (int uiSlot) =>
//        {
//            bool found = inventoryUIMapper.TryGetMappingItemSlot(uiSlot, out ItemId itemId, out int slotIndex);

//            if (hasPutItem)
//            {
//                inventoryUIMapper.SwapSlot(putUISlot, uiSlot);
//                Clear();
//            }
//            else if (found)
//            {
//                putItemId = itemId;
//                hasPutItem = true;
//                putUISlot = uiSlot;
//            }
//        };

//        hotKey.ActionHotKeyClick += (EHotKeyType hotKeyType) =>
//        {
//            if (hasPutItem)
//            {
//                ItemData item = CharacterInven.ItemManager.Get(putItemId);
//                ItemHotKey itemHotKey = new ItemHotKey(putItemId, item.Name, item.Icon, CharacterInven);
//                hotKey.SetSlot(hotKeyType, itemHotKey);

//                itemHotKeyMappings[putItemId] = hotKeyType;
//                Clear();
//            }
//            else
//            {
//                if (prevHotKeyType == EHotKeyType.Error)
//                    prevHotKeyType = hotKeyType;
//                else
//                {
//                    hotKey.SwapSlot(prevHotKeyType, hotKeyType);
//                    Clear();
//                }
//            }
//        };

//        CharacterInven.Inventory.OnSlotRemove += (int slotIndex, ItemId itemId) =>
//        {
//            if (itemHotKeyMappings.TryGetValue(itemId, out EHotKeyType hotKeyType))
//            {
//                hotKey.SetSlot(hotKeyType, null);
//                itemHotKeyMappings.Remove(itemId);
//            }
//        };
//    }

//    private void Clear()
//    {
//        hasPutItem = false;
//        putItemId = default;
//        putUISlot = -1;
//        prevHotKeyType = EHotKeyType.Error;
//    }

//    private bool hasPutItem = false;
//    private ItemId putItemId;
//    private int putUISlot = -1;

//    private EHotKeyType prevHotKeyType = EHotKeyType.Error;

//    private Dictionary<ItemId, EHotKeyType> itemHotKeyMappings = new();
//}
