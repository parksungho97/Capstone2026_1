using System;
using System.Collections;
using Fusion;
using UnityEngine;

public class Npc : NetworkBehaviour
{
    public static event Action<Npc> OnNetworkSpawned;
    [SerializeField] private ItemDropper itemDropper;
    [SerializeField] private float deathDespawnDelay = 10f;

    private VoicePlayer voicePlayer;
    private NpcMove npcMove;
    private Animator animator;

    [Networked] public bool bMoving { get; private set; }
    [Networked] public NetworkBool IsDead { get; private set; }

    public override void Spawned()
    {
        base.Spawned();

        Debug.Assert(itemDropper);

        voicePlayer = GetComponent<VoicePlayer>();
        npcMove = GetComponent<NpcMove>();
        animator = GetComponentInChildren<Animator>();

        Debug.Assert(voicePlayer);
        Debug.Assert(npcMove);

        OnNetworkSpawned?.Invoke(this);
    }

    public void DestroyNpc()
    {
        if (!Object.HasStateAuthority)
            return;

        if (IsDead)
            return;

        IsDead = true;

        if (npcMove != null)
            npcMove.StopMove();

        RPC_OnDead();

        StartCoroutine(DespawnAfterDeath());
    }

    private IEnumerator DespawnAfterDeath()
    {
        yield return new WaitForSeconds(deathDespawnDelay);

        itemDropper.Drop();
        Runner.Despawn(Object);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnDead()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.SetTrigger("Die");

        if (voicePlayer != null)
            voicePlayer.DisableAfterPlaying();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority)
            bMoving = !IsDead && npcMove.IsMoving;
    }

    public override void Render()
    {
        base.Render();

        if (voicePlayer == null)
            return;

        if (IsDead)
        {
            voicePlayer.StopFootSteps();
            return;
        }

        if (bMoving)
            voicePlayer.StartFootSteps();
        else
            voicePlayer.StopFootSteps();
    }
}