using Fusion;
using System;
using UnityEngine;

public class MeleeAttackInstance : MonoBehaviour
{
    private int damage;
    private float knockbackForce;
    private float activationTime;
    private int vfxId;
    private int hitSoundId;
    private float hitStunDuration;

    private Collider hitCollider;
    private NetworkObject self;
    private NetworkObject owner;
    private Vector3 spawnOffset;
    private float activationTimer;
    private bool activated;
    private bool despawning;

    public Action ActionDestroy;

    public void Init(NetworkObject self, NetworkObject owner, MeleeAttackData data)
    {
        this.self = self;
        this.owner = owner;
        spawnOffset = data.SpawnOffset;
        damage = data.Damage;
        knockbackForce = data.KnockbackForce;
        activationTime = data.ActivationTime;
        vfxId = data.HitVfxId;
        hitSoundId = data.HitSoundId;
        hitStunDuration = data.HitStunDuration;

        hitCollider = GetComponent<Collider>();
        if (hitCollider != null) hitCollider.enabled = activationTime <= 0f;

        activationTimer = activationTime;
    }

    private void DoDespawn()
    {
        despawning = true;
        ActionDestroy?.Invoke();
        self.Runner.Despawn(self);
    }

    private void FixedUpdate()
    {
        if (!self.HasStateAuthority || despawning) return;

        if (activated)
        {
            DoDespawn();
            return;
        }

        if (activationTimer > 0f)
        {
            if (owner != null)
            {
                transform.position = owner.transform.position + owner.transform.rotation * spawnOffset;
                transform.rotation = owner.transform.rotation;
            }

            activationTimer -= Time.fixedDeltaTime;
            if (activationTimer <= 0f && hitCollider != null)
            {
                hitCollider.enabled = true;
                activated = true;
            }
            return;
        }

        // activationTime <= 0: collider already enabled from Init, mark for despawn next tick
        activated = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!self.HasStateAuthority || despawning)
            return;

        if (owner == null || other.transform.IsChildOf(owner.transform))
            return;

        HitComponent hit = other.GetComponent<HitComponent>();
        if (hit)
        {
            Vector3 diff = other.transform.position - owner.transform.position;
            diff.y = 0f;
            Vector3 dir = diff.normalized;
            hit.Hit(damage, dir, knockbackForce, vfxId, hitSoundId, hitStunDuration);
            DoDespawn();
        }
    }
}
