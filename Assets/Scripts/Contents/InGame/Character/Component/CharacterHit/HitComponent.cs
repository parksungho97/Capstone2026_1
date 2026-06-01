using Fusion;
using System;
using UnityEngine;

public class HitComponent : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterHealth health;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float hitStunDuration = 0.5f;
    [SerializeField] private VoicePlayer voicePlayer;

    public Action ActionHitted;

    private void Start()
    {
        Debug.Assert(rb);
        Debug.Assert(health);
    }

    public void Hit(int damage, Vector3 knockbackDir, float knockbackForce, int vfxId = -1, int soundId = -1)
    {
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
            playerController?.DisableInputForSeconds(hitStunDuration);

        if (vfxId >= 0)
            VFXManager.Instance.Spawn(vfxId, transform.position);
        if (soundId >= 0)
            voicePlayer?.PlayAttackClip(soundId);

        ActionHitted?.Invoke();
    }
}
