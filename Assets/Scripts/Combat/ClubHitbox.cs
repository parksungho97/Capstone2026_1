using UnityEngine;
using UnityEngine.Events;

public class ClubHitbox : MonoBehaviour
{
    [Header("Owner")]
    public GameObject owner;

    [Header("Combat Values")]
    public int damage = 34;
    public float knockbackForce = 8f;
    public float stunDuration = 0.3f;

    [Header("Events")]
    public UnityEvent<GameObject> onHitTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;
        if (other.gameObject == owner) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log($"[ClubHitbox] {owner.name} hit {other.gameObject.name}");

        onHitTarget?.Invoke(other.gameObject);
    }
}