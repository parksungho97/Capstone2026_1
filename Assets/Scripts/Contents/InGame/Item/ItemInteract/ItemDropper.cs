using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    public class DropData
    {
        public ItemId ItemId;
        public int Count;
    }

    private readonly List<DropData> dropDataList = new();

    public void SetDropData(ItemId itemId, int count)
    {
        dropDataList.Add(new DropData { ItemId = itemId, Count = count });
    }

    public void Drop()
    {
        if (ItemInstanceManager.Instance == null) return;

        DropData data = dropDataList[Random.Range(0, dropDataList.Count)];
        ItemInstanceManager.Instance.RPC_RequestSpawn(data.ItemId.Value, transform.position, data.Count);
    }
}
