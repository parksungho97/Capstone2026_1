using Fusion;
using UnityEngine;

public class NpcAttackNetworkState : NetworkBehaviour
{
    [SerializeField] private int meleeAttackId = 0;
    [SerializeField] private float attackInterval = 1.5f;

    [SerializeField] private Animator animator;

    public AttackDelay AttackDelay { get; } = new AttackDelay();

    private Attacker attacker;

    public override void Spawned()
    {
        base.Spawned();

        attacker = GetComponent<Attacker>();
        Debug.Assert(attacker);

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        AttackDelay.Tick(Runner.DeltaTime);
    }

    public void Attack()
    {
        if (!Object.HasStateAuthority)
            return;

        if (!AttackDelay.IsAttackReady())
            return;

        attacker.MeleeAttack(meleeAttackId, AttackDelay);
        AttackDelay.SetDelay(attackInterval);

        RPC_OnAttack();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnAttack()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.SetTrigger("Attack");
    }
}