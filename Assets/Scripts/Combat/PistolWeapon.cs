using System.Collections;
using UnityEngine;

public class PistolWeapon : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.35f;

    private bool canAttack = true;

    public void TryAttack()
    {
        if (!canAttack) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        Fire();

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    private void Fire()
    {
        if (firePoint == null)
        {
            Debug.LogError("[PistolWeapon] FirePoint가 연결되지 않았습니다.");
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("[PistolWeapon] BulletPrefab이 연결되지 않았습니다.");
            return;
        }

        GameObject bulletObject = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        PistolBullet bullet = bulletObject.GetComponent<PistolBullet>();

        if (bullet != null)
        {
            bullet.SetOwner(gameObject);
        }

        Debug.Log("[PistolWeapon] 총알 발사");
    }

    public void CancelAttack()
    {
        StopAllCoroutines();
        canAttack = true;

        Debug.Log("[PistolWeapon] 공격 취소");
    }
}