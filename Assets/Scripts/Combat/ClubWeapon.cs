using System.Collections;
using UnityEngine;

public class ClubWeapon : MonoBehaviour
{
    [Header("Hitbox")]
    public Collider clubHitbox;

    [Header("Attack Settings")]
    public float hitboxActiveTime = 0.15f;
    public float attackCooldown = 0.6f;

    private bool canAttack = true;

    private void Awake()
    {
        if (clubHitbox != null)
            clubHitbox.enabled = false;
    }

    public void TryAttack()
    {
        if (!canAttack) return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        if (clubHitbox != null)
            clubHitbox.enabled = true;

        yield return new WaitForSeconds(hitboxActiveTime);

        if (clubHitbox != null)
            clubHitbox.enabled = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void CancelAttack()
    {
        StopAllCoroutines();

        if (clubHitbox != null)
            clubHitbox.enabled = false;

        canAttack = true;
    }
}