using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunAttackInstance : MonoBehaviour
{
    private int damage;
    private float knockbackForce;
    private float activationTime;
    private float duration;
    private int vfxId;
    private int hitSoundId;
    private Vector3 initialScale;
    private Vector3 finalScale;
    private float dashSpeed;
    private Vector3 dashDir;
    private Vector3 spawnOffset;
    private Quaternion spawnRotation;

    private Collider hitCollider;
    private Rigidbody ownerRb;
    private NetworkObject self;
    private NetworkObject owner;
    private float activationTimer;
    private float durationTimer;
    private readonly HashSet<HitComponent> hitTargets = new();

    public Action ActionDestroy;

    public void Init(NetworkObject self, NetworkObject owner, ShotgunAttackData data)
    {
        this.self = self;
        this.owner = owner;
        damage = data.Damage;
        knockbackForce = data.KnockbackForce;
        activationTime = data.ActivationTime;
        duration = data.Duration;
        vfxId = data.HitVfxId;
        hitSoundId = data.HitSoundId;
        initialScale = data.InitialScale;
        finalScale = data.FinalScale;
        dashSpeed = data.DashSpeed;
        dashDir = owner.transform.forward;
        spawnOffset = data.SpawnOffset;
        spawnRotation = owner.transform.rotation;

        hitCollider = GetComponent<Collider>();
        ownerRb = owner.GetComponent<Rigidbody>();

        transform.localScale = initialScale;
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
            activationTimer -= Time.fixedDeltaTime;
            if (activationTimer <= 0f && hitCollider != null)
                hitCollider.enabled = true;
            return;
        }

        transform.position += dashDir * dashSpeed * Time.fixedDeltaTime;


        durationTimer -= Time.fixedDeltaTime;

        float t = Mathf.Clamp01(1f - (durationTimer / duration));
        transform.localScale = new Vector3(
            Mathf.Lerp(initialScale.x, finalScale.x, t),
            initialScale.y,
            Mathf.Lerp(initialScale.z, finalScale.z, t)
        );

        if (durationTimer <= 0f)
        {
            ActionDestroy?.Invoke();
            self.Runner.Despawn(self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!self.HasStateAuthority || activationTimer > 0f) return;
        if (owner != null && other.transform.IsChildOf(owner.transform)) return;

        HitComponent hit = other.GetComponent<HitComponent>();
        if (hit == null || hitTargets.Contains(hit)) return;

        hitTargets.Add(hit);
        Vector3 dir = (other.transform.position - transform.position).normalized;
        hit.Hit(damage, dir, knockbackForce, vfxId, hitSoundId);
    }
}
