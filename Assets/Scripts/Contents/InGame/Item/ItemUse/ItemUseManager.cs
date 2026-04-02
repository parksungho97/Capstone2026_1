using System.Collections.Generic;
using UnityEngine;

public enum EItemUseType : int
{
    Heal,
    End,
}

public abstract class ItemUsable
{
    public ItemUsable(EItemUseType itemUseType)
    {
        this.ItemUseType = itemUseType;
    }
    public EItemUseType ItemUseType {  get; private set; }
}

public class ItemUseData : ItemData
{
    public ItemUseData(int id, string itemName, string description, Sprite icon, int maxCount, EItemUseType useType) 
        : base(id, itemName, description, icon, EItemType.Usable, maxCount)
    {
        this.ItemUseType = useType;
    }

    public EItemUseType ItemUseType { get; private set; }
}

public abstract class ItemUseSO : ItemLoadData
{
    public EItemUseType ItemUseType;
}

public class ItemUseManager
{
    private static ItemUseManager instance;
    public static ItemUseManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ItemUseManager();
            return instance;
        }
    }

    public T GetItemUse<T>(EItemUseType itemType) where T : ItemUsable
    {
        return itemUsables[(int)itemType] as T;
    }

    public void UseItem(ItemUseData itemUseData)
    {
        switch(itemUseData.ItemUseType)
        {
            case EItemUseType.Heal:
                HealData healData = (HealData)itemUseData;
                HealEffect healEffect = GetItemUse<HealEffect>(EItemUseType.Heal);
                Debug.Assert(healEffect != null, "HealEffect is not initialized.");
                healEffect.Heal(healData);
                break;
            case EItemUseType.End:
                break;
            default:
                break;
        }
    }

    public void Initalize()
    {
        // 여기서 다 초기화

        itemUsables[(int)EItemUseType.Heal] = new HealEffect();

        for (int i = 0; i < (int)EItemUseType.End; ++i)
        {
            Debug.Assert(itemUsables[i] != null, $"ItemUsable for {((EItemUseType)i).ToString()} is not initialized.");
            Debug.Assert(itemUsables[i].ItemUseType == (EItemUseType)i);
        }
    }

    private ItemUsable[] itemUsables = new ItemUsable[(uint)EItemUseType.End];
}
