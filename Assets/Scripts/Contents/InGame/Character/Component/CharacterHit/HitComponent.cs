using Fusion;
using System;
using UnityEngine;

public class HitComponent : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterHealth health;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float invincibleDuration = 1.5f;
    [SerializeField] private VoicePlayer voicePlayer;
    [SerializeField] private InvincibilityBlinker blinker;
    [SerializeField] private NpcMove npcMove;
    [SerializeField] private CharacterAttack characterAttack;
    [SerializeField] private NpcAttackNetworkState npcAttackNetworkState;

    public Action ActionHitted;

    private float invincibilityEndTime = -1f;

    public void SetInvincible(float duration = -1f)
    {
        float dur = duration < 0f ? invincibleDuration : duration;
        invincibilityEndTime = Time.time + dur;
        blinker?.StartBlink(dur);
    }


    private void Start()
    {
        Debug.Assert(rb);
        Debug.Assert(health);
        Debug.Assert(blinker);
    }

    public void Hit(int damage, Vector3 knockbackDir, float knockbackForce, int vfxId = -1, int soundId = -1, float stunDuration = 0f)
    {
        if (Time.time < invincibilityEndTime)
            return;
        RPC_Hit(damage, knockbackDir, knockbackForce, vfxId, soundId, stunDuration);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Hit(int damage, Vector3 knockbackDir, float knockbackForce, int vfxId, int soundId, float stunDuration)
    {
        Debug.Log($"[HitComponent] Hit received — damage: {damage}");

        if (Object.HasStateAuthority)
        {
            Vector3 knockback = knockbackDir.normalized * knockbackForce;
            if (npcMove != null)
                npcMove.Knockback(knockback);
            else if (rb)
                rb.velocity = knockback;
            health?.ApplyDamage(damage);
        }

        if (Object.HasInputAuthority)
        {
            playerController?.DisableInputForSeconds(stunDuration);
            AttackDelay attackDelay = characterAttack != null
                ? characterAttack.AttackContext.AttackDelay
                : npcAttackNetworkState?.AttackDelay;
            if (attackDelay != null && attackDelay.Timer < stunDuration)
                attackDelay.SetDelay(stunDuration);
        }

        if (vfxId >= 0)
            VFXManager.Instance.Spawn(vfxId, transform.position);
        if (soundId >= 0)
            voicePlayer?.PlayAttackClip(soundId);

        ActionHitted?.Invoke();
    }
}
