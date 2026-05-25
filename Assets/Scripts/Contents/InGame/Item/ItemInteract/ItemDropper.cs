using Fusion;
using Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ItemDropper : MonoBehaviour
{
    [SerializeField] private NetworkObject itemInstancePrefab;
    public class DropData
    {
        public ItemId ItemId;
        public int Count;
    }

    [SerializeField] private float launchForce = 4f;
    [SerializeField] private float scatterRadius = 1.5f;

    private readonly List<DropData> dropDataList = new();

    public void SetDropData(ItemId itemId, int count)
    {
        dropDataList.Add(new DropData { ItemId = itemId, Count = count });
    }

    public void Drop()
    {
        NetworkRunner runner = NetworkRoot.Instance.Runner;
        if (!runner.IsRunning) return;

        DropData data = dropDataList[UnityEngine.Random.Range(0, dropDataList.Count)];

        if (!ItemManager.Instance.TryGet(data.ItemId, out ItemData itemData) )
        {
            Debug.LogWarning($"[ItemDropper] {data.ItemId}에 FieldPrefab이 없습니다.");
            return;
        }

        _ = SpawnItemAsync(runner, data);
    }

    private async Task SpawnItemAsync(NetworkRunner runner, DropData data)
    {
        Vector2 scatter = UnityEngine.Random.insideUnitCircle * scatterRadius;

        var obj = await runner.SpawnAsync(
            itemInstancePrefab,
            transform.position,
            Quaternion.identity,
            onBeforeSpawned: (r, networkObj) =>
            {
                networkObj.GetComponent<ItemInstance>().SetItemData(data.ItemId, data.Count);
            }
        );

        if (obj != null && obj.TryGetComponent(out Rigidbody rb))
            rb.AddForce(new Vector3(scatter.x, launchForce, scatter.y), ForceMode.Impulse);
    }
}
