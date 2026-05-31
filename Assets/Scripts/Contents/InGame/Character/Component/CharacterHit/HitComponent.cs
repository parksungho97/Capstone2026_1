using Fusion;
using System;
using UnityEngine;

public class HitComponent : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CharacterHealth health;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float hitStunDuration = 0.5f;

    public Action ActionHitted;

    [SerializeField] private VoicePlayer voicePlayer;

    private void Start()
    {
        Debug.Assert(rb);
        Debug.Assert(health);
    }

    public void Hit(int damage, Vector3 knockbackDir, float knockbackForce, int vfxId = -1, int soundId = -1)
    {
        RPC_HitAuthority(damage, knockbackDir, knockbackForce);
        RPC_HitAll(vfxId, soundId);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_HitAuthority(int damage, Vector3 knockbackDir, float knockbackForce)
    {
        if (rb)
            rb.velocity = knockbackDir.normalized * knockbackForce;
        health?.RPC_ServeHP(damage);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_HitAll(int vfxId, int soundId)
    {
        playerController?.DisableInputForSeconds(hitStunDuration);

        if (vfxId >= 0)
            VFXManager.Instance.Spawn(vfxId, transform.position);
        if (soundId >= 0)
            voicePlayer?.PlayAttackClip(soundId);
        ActionHitted?.Invoke();
    }
}
