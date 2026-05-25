using UnityEngine;

public class HitComponent : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [SerializeField] private CharacterHealth health;

    private void Start()
    {
        Debug.Assert(rb);
        Debug.Assert(health);
    }

    public void Hit(int damage, Vector3 knockbackDir, float knockbackForce)
    {
        rb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode.Impulse);

        health.RPC_ServeHP(damage);
    }
}
