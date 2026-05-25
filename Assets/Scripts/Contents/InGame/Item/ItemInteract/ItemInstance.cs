using Fusion;
using UnityEngine;

public class ItemInstance : NetworkBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    [Networked] public int ItemIdValue { get; set; }
    [Networked] public int Count { get; set; }

    public ItemId ItemId => new ItemId(ItemIdValue);

    private void Start()
    {
        Debug.Assert(meshFilter);
        Debug.Assert(meshRenderer);
    }

    public void SetItemData(ItemId id, int count)
    {
        if (!Object.HasStateAuthority) return;

        ItemIdValue = id.Value;
        Count = count;

        UpdateVisual();
    }

    public override void Render()
    {
        base.Render();

        if (ItemIdValue != 0)
            UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (ItemManager.Instance.TryGet(ItemId, out ItemData data))
        {
            // 최적화와 에러 방지를 위해, 이미 세팅된 메쉬/재질과 똑같다면 갱신을 스킵합니다.
            if (meshFilter.sharedMesh == data.Mesh && meshRenderer.sharedMaterial == data.Material)
                return;

            meshFilter.mesh = data.Mesh;
            meshRenderer.material = data.Material;
        }
    }

    public void Take(InventoryController controller)
    {
        // 내 인벤토리에 아이템을 넣는 연산
        controller.AddItem(ItemId, Count);

        // 서버에게 이 오브젝트를 파괴(Despawn)해달라고 RPC 요청
        RPC_Despawn();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Despawn()
    {
        Runner.Despawn(Object);
    }
}