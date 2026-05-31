using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class NpcAnimationNetworkState : NetworkBehaviour
{
    [Networked] public NetworkBool IsMoving { get; private set; }
    [Networked] public NetworkBool IsHit { get; private set; }

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private HitComponent hitComponent;
    [SerializeField] private float moveThreshold = 0.05f;

    private void Awake()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (hitComponent == null)
            hitComponent = GetComponentInChildren<HitComponent>();
    }

    public override void Spawned()
    {
        if (hitComponent != null)
            hitComponent.ActionHitted += OnHit;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (hitComponent != null)
            hitComponent.ActionHitted -= OnHit;
    }

    private void OnHit()
    {
        SetHit(true);
    }

    public void SetHit(bool isHit)
    {
        if (!Object.HasStateAuthority)
            return;

        IsHit = isHit;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (navMeshAgent == null)
            return;

        IsMoving = navMeshAgent.velocity.sqrMagnitude > moveThreshold * moveThreshold;
    }
}