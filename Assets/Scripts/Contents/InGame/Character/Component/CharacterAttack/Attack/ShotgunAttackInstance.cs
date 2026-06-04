using Fusion;
using System;
using UnityEngine;

public class ShotgunAttackInstance : MonoBehaviour
{
    private int damage;
    private float knockbackForce;
    private float speed;
    private float maxDistance;
    private int vfxId;
    private int hitSoundId;
    private float hitStunDuration;
    private LayerMask obstacleLayer;
    private float activationTimer;

    private NetworkObject self;
    private NetworkObject owner;
    private Vector3 startPosition;
    private bool despawning;
    private Collider hitCollider;

    public Action ActionDestroy;

    public void Init(NetworkObject self, NetworkObject owner, ShotgunAttackData data)
    {
        this.self = self;
        this.owner = owner;
        damage = data.Damage;
        knockbackForce = data.KnockbackForce;
        speed = data.Speed;
        maxDistance = data.MaxDistance;
        vfxId = data.HitVfxId;
        hitSoundId = data.HitSoundId;
        hitStunDuration = data.HitStunDuration;
        obstacleLayer = data.ObstacleLayer;
        activationTimer = data.ActivationTime;
        startPosition = transform.position;
        despawning = false;

        hitCollider = GetComponent<Collider>();
        if (hitCollider != null)
            hitCollider.enabled = activationTimer <= 0f;
    }

    private void FixedUpdate()
    {
        if (!self.HasStateAuthority || despawning) return;

        if (activationTimer > 0f)
        {
            activationTimer -= Time.fixedDeltaTime;
            if (activationTimer <= 0f && hitCollider != null)
                hitCollider.enabled = true;
            return;
        }

        transform.position += transform.forward * speed * Time.fixedDeltaTime;

        if (Vector3.Distance(transform.position, startPosition) >= maxDistance)
            DoDespawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!self.HasStateAuthority || despawning) return;
        if (owner != null && other.transform.IsChildOf(owner.transform)) return;

        if ((obstacleLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            DoDespawn();
            return;
        }

        HitComponent hit = other.GetComponent<HitComponent>();
        if (hit != null)
        {
            Vector3 dir = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
            hit.Hit(damage, dir, knockbackForce, vfxId, hitSoundId, hitStunDuration);
            DoDespawn();
        }
    }

    private void DoDespawn()
    {
        despawning = true;
        ActionDestroy?.Invoke();
        self.Runner.Despawn(self);
    }
}
