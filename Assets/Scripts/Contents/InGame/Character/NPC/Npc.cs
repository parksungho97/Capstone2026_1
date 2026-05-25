using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class Npc : NetworkBehaviour
{
    [SerializeField] private ItemDropper itemDropper;

    private void Start()
    {
        Debug.Assert(itemDropper);
    }

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority == false)
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
                navMeshAgent.enabled = false;
        }
    }

    public void DestroyNpc()
    {
        if (!Object.HasStateAuthority) return;

        itemDropper.Drop();

        Runner.Despawn(Object);
    }
}