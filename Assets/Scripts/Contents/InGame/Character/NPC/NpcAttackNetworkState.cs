using Fusion;
using UnityEngine;

public class NpcAttackNetworkState : NetworkBehaviour
{
    [SerializeField] private int meleeAttackId = 0;
    [SerializeField] private float attackInterval = 1.5f;

    private Attacker attacker;
    private readonly AttackDelay attackDelay = new AttackDelay();

    public override void Spawned()
    {
        base.Spawned();
        attacker = GetComponent<Attacker>();
        Debug.Assert(attacker);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) 
            return;
        
        attackDelay.Tick(Runner.DeltaTime);
    }

    public void Attack()
    {
        if (!Object.HasStateAuthority) 
            return;
        if (!attackDelay.IsAttackReady()) 
            return;

        attacker.MeleeAttack(meleeAttackId, attackDelay);
        attackDelay.SetDelay(attackInterval);
        RPC_OnAttack();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnAttack()
    {
        Debug.Log("NpcAttack!");
    }
}
