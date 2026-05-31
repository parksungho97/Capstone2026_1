using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class ItemInstanceManager : NetworkBehaviour
{
    [SerializeField] private ItemInstance itemInstancePrefab;
    public static ItemInstanceManager Instance { get; private set; }

    // Host-only: managingId → itemId
    private readonly Dictionary<int, int> managingIdToItemId = new();

    // Per-client: bidirectional managingId ↔ ItemInstance
    private readonly Dictionary<int, ItemInstance> managingIdToInstance = new();
    private readonly Dictionary<ItemInstance, int> instanceToManagingId = new();

    private int nextManagingId = 0;

    public override void Spawned()
    {
        base.Spawned();
        Instance = this;

        Debug.Assert(itemInstancePrefab);

        // 씬에 직접 배치된 ItemInstance를 계층 구조 순서 기준으로 정렬해 등록
        // (모든 클라이언트가 동일한 씬을 로드하므로 동일한 순서 보장)
        var sceneItems = FindObjectsByType<ItemInstance>(FindObjectsSortMode.None)
            .OrderBy(item => GetHierarchyPath(item.transform))
            .ToList();

        foreach (var item in sceneItems)
        {
            int managingId = nextManagingId++;
            Register(managingId, item);
            if (Object.HasStateAuthority)
                managingIdToItemId[managingId] = item.ItemIdValue;
        }
    }

    private static string GetHierarchyPath(Transform t)
    {
        var path = new System.Text.StringBuilder();
        while (t != null)
        {
            path.Insert(0, "/" + t.GetSiblingIndex().ToString("D6"));
            t = t.parent;
        }
        return path.ToString();
    }

    // Any peer requests a new item spawn; only the host processes it
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestSpawn(int itemId, Vector3 position, int count)
    {
        int managingId = nextManagingId++;
        managingIdToItemId[managingId] = itemId;
        RPC_OnItemSpawned(managingId, itemId, position, count);
    }

    // Host broadcasts; every client spawns and registers the instance locally
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnItemSpawned(int managingId, int itemId, Vector3 position, int count)
    {
        // Spawn the prefab for itemId at position, then register it
        // (No prefab assigned per ID yet — uncomment when ready)
        ItemId itemIdInst = new ItemId(itemId);
        if(ItemManager.Instance.TryGet(itemIdInst, out ItemData itemData))
        {
            ItemInstance instance = Instantiate(itemInstancePrefab, position, Quaternion.identity);
            instance.SetItemData(itemIdInst, count);
            if (itemData.ItemPrefab != null)
                Instantiate(itemData.ItemPrefab, instance.transform);
            Register(managingId, instance);
        }
    }

    // Called by ItemInstance.Take; resolves managingId and broadcasts destruction
    public void RequestDestroy(ItemInstance instance)
    {
        if (!instanceToManagingId.TryGetValue(instance, out int managingId))
            return;

        RPC_RequestDestroy(managingId);
    }

    // Every client destroys the local instance for this managingId
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RequestDestroy(int managingId)
    {
        if (!managingIdToInstance.TryGetValue(managingId, out ItemInstance instance))
            return;

        managingIdToInstance.Remove(managingId);
        instanceToManagingId.Remove(instance);

        if (instance != null)
            Destroy(instance.gameObject);
    }

    private void Register(int managingId, ItemInstance instance)
    {
        managingIdToInstance[managingId] = instance;
        instanceToManagingId[instance] = managingId;
    }
}
