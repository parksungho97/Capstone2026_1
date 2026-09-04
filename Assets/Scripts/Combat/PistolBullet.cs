using UnityEngine;
using UnityEngine.Events;

public class PistolBullet : MonoBehaviour
{
    [Header("Owner")]
    [SerializeField] private GameObject owner;

    [Header("Bullet Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifeTime = 2f;

    [Header("Hit Event")]
    public UnityEvent<GameObject> onHitTarget;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null && other.gameObject == owner) return;

        if (!other.CompareTag("Player")) return;

        Debug.Log($"[PistolBullet] ≈∏∞Ÿ ∏Ì¡ﬂ / target: {other.gameObject.name}, damage: {damage}");

        onHitTarget?.Invoke(other.gameObject);

        Destroy(gameObject);
    }

    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    public int GetDamage()
    {
        return damage;
    }

    public float GetSpeed()
    {
        return speed;
    }
}