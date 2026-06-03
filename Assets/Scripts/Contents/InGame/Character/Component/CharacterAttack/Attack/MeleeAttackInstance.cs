using Fusion;
using System;
using UnityEngine;

public class MeleeAttackInstance : MonoBehaviour
{
    private int damage;
    private float knockbackForce;
    private float activationTime;
    private float duration;
    private int vfxId;
    private int hitSoundId;
    private float hitStunDuration;

    private Collider hitCollider;
    private NetworkObject self;
    private NetworkObject owner;
    private Vector3 spawnOffset;
    private float activationTimer;
    private float durationTimer;

    public Action ActionDestroy;

    public void Init(NetworkObject self, NetworkObject owner, MeleeAttackData data)
    {
        this.self = self;
        this.owner = owner;
        spawnOffset = data.SpawnOffset;
        damage = data.Damage;
        knockbackForce = data.KnockbackForce;
        activationTime = data.ActivationTime;
        duration = data.Duration;
        vfxId = data.HitVfxId;
        hitSoundId = data.HitSoundId;
        hitStunDuration = data.HitStunDuration;

        hitCollider = GetComponent<Collider>();
        if (hitCollider != null) hitCollider.enabled = false;

        activationTimer = activationTime;
        durationTimer = duration;
        if (hitCollider != null)
            hitCollider.enabled = activationTime <= 0f;
    }

    private void FixedUpdate()
    {
        if (!self.HasStateAuthority) return;

        if (activationTimer > 0f)
        {
            if (owner != null)
            {
                transform.position = owner.transform.position + owner.transform.rotation * spawnOffset;
                transform.rotation = owner.transform.rotation;
            }

            activationTimer -= Time.fixedDeltaTime;
            if (activationTimer <= 0f && hitCollider != null)
                hitCollider.enabled = true;
            return;
        }

        durationTimer -= Time.fixedDeltaTime;
        if (durationTimer <= 0f)
        {
            ActionDestroy?.Invoke();
            self.Runner.Despawn(self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!self.HasStateAuthority || activationTimer > 0f) 
            return;

        if (owner == null || other.transform.IsChildOf(owner.transform)) 
            return;

        HitComponent hit = other.GetComponent<HitComponent>();
        if (hit)
        {
            Vector3 dir = (other.transform.position - transform.position).normalized;
            hit.Hit(damage, dir, knockbackForce, vfxId, hitSoundId, hitStunDuration);
        }
    }
}
