using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    public class DropData
    {
        public ItemId ItemId;
    }

    private readonly List<DropData> dropDataList = new();

    public void SetDropData(ItemId itemId)
    {
        dropDataList.Add(new DropData { ItemId = itemId });
    }

    public void Drop()
    {
        if (ItemInstanceManager.Instance == null) return;

        DropData data = dropDataList[Random.Range(0, dropDataList.Count)];

        int spawnCount = 1;
        if (ItemManager.Instance != null && ItemManager.Instance.TryGet(data.ItemId, out ItemData itemData))
            spawnCount = itemData.SpawnCount;

        ItemInstanceManager.Instance.SpawnImmediate(data.ItemId.Value, transform.position, spawnCount);
    }
}
