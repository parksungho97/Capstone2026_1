using Fusion;
using System;
using UnityEngine;

public class HitComponent : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterHealth health;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float hitStunDuration = 0.5f;
    [SerializeField] private float invincibleDuration = 1.5f;
    [SerializeField] private VoicePlayer voicePlayer;
    [SerializeField] private InvincibilityBlinker blinker;

    public Action ActionHitted;

    private float invincibilityEndTime = -1f;

    public void SetInvincible(float duration = -1f)
    {
        float dur = duration < 0f ? invincibleDuration : duration;
        invincibilityEndTime = Time.time + dur;
        blinker?.StartBlink(dur);
    }

    private CharacterAttack characterAttack;

    private void Start()
    {
        Debug.Assert(rb);
        Debug.Assert(health);
        Debug.Assert(blinker);
        characterAttack = GetComponent<CharacterAttack>();
    }

    public void Hit(int damage, Vector3 knockbackDir, float knockbackForce, int vfxId = -1, int soundId = -1)
    {
        if (Time.time < invincibilityEndTime)
            return;
        RPC_Hit(damage, knockbackDir, knockbackForce, vfxId, soundId);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_Hit(int damage, Vector3 knockbackDir, float knockbackForce, int vfxId, int soundId)
    {
        Debug.Log($"[HitComponent] Hit received — damage: {damage}");

        if (Object.HasStateAuthority)
        {
            if (rb) rb.velocity = knockbackDir.normalized * knockbackForce;
            health?.ApplyDamage(damage);
        }

        if (Object.HasInputAuthority)
        {
            playerController?.DisableInputForSeconds(hitStunDuration);
            AttackDelay attackDelay = characterAttack?.AttackContext.AttackDelay;
            if (attackDelay != null && attackDelay.Timer < hitStunDuration)
                attackDelay.SetDelay(hitStunDuration);
        }

        if (vfxId >= 0)
            VFXManager.Instance.Spawn(vfxId, transform.position);
        if (soundId >= 0)
            voicePlayer?.PlayAttackClip(soundId);

        ActionHitted?.Invoke();
    }
}
