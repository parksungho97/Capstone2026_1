using Fusion;
using System;
using UnityEngine;

public class ProjectileInstance : MonoBehaviour
{
    private int damage;
    private float knockbackForce;
    private float speed;
    private float maxDistance;

    private NetworkObject self;
    private NetworkObject owner;
    private Vector3 startPosition;
    private bool despawning;

    public Action ActionDestroy;

    public void Init(NetworkObject self, NetworkObject owner, RangedAttackData data)
    {
        this.self = self;
        this.owner = owner;
        damage = data.Damage;
        knockbackForce = data.KnockbackForce;
        speed = data.Speed;
        maxDistance = data.MaxDistance;
        startPosition = transform.position;
        despawning = false;
    }

    private void FixedUpdate()
    {
        if (!self.HasStateAuthority || despawning) return;

        transform.position += transform.forward * speed * Time.fixedDeltaTime;

        if (Vector3.Distance(transform.position, startPosition) >= maxDistance)
            DoDespawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!self.HasStateAuthority || despawning) return;
        if (owner != null && other.transform.IsChildOf(owner.transform)) return;

        HitComponent hit = other.GetComponent<HitComponent>();
        if (hit)
        {
            Debug.Log(other.name);
            hit.Hit(damage, transform.forward, knockbackForce);
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
