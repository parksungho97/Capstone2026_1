using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class NpcAnimationNetworkState : NetworkBehaviour
{
    [Networked] public NetworkBool IsMoving { get; private set; }

    [SerializeField] private Npc npc;
    [SerializeField] private HitComponent hitComponent;
    [SerializeField] private float moveThreshold = 0.05f;
    public bool IsHit { get; private set; }

    private void Awake()
    {
        if (hitComponent == null)
            hitComponent = GetComponentInChildren<HitComponent>();
    }

    public override void Spawned()
    {
        if (hitComponent != null)
            hitComponent.ActionHitted += OnHit;
        npc = GetComponent<Npc>();
        Debug.Assert(npc);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (hitComponent != null)
            hitComponent.ActionHitted -= OnHit;
    }

    private void OnHit()
    {
        Debug.Log("NpcHitted");
        SetHit(true);
    }

    public void SetHit(bool isHit)
    {
        IsHit = isHit;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if(npc.bMoving)
        {
            // Tood: 
        }
    }
}