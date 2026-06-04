using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcMove : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 10f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        voicePlayer = GetComponent<VoicePlayer>();
        Debug.Assert(navMeshAgent);
        Debug.Assert(voicePlayer);
        CenterPos = transform.position;
    }

    public bool IsMoving => navMeshAgent.enabled && navMeshAgent.velocity.sqrMagnitude > 0.01f;
    public Vector3 CenterPos { get; set; }

    public bool IsReach()
    {
        if (!navMeshAgent.enabled) return false;
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f;
    }

    public void SetRandomDestination()
    {
        if (!navMeshAgent.enabled) return;
        NavMeshHit hit;
        Vector3 randomPoint = CenterPos + Random.insideUnitSphere * wanderRadius;
        randomPoint.y = CenterPos.y;

        if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            navMeshAgent.SetDestination(hit.position);
    }

    public void SetDestination(Vector3 destination)
    {
        if (!navMeshAgent.enabled) return;
        navMeshAgent.SetDestination(destination);
    }

    public void StopMove()
    {
        if (!navMeshAgent.enabled) return;
        navMeshAgent.ResetPath();
    }

    public void Knockback(Vector3 velocity)
    {
        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);
        knockbackRoutine = StartCoroutine(KnockbackRoutine(velocity));
    }

    private IEnumerator KnockbackRoutine(Vector3 velocity)
    {
        navMeshAgent.enabled = false;
        //rb.useGravity = false;
        //rb.isKinematic = true;
        rb.velocity = velocity;
        yield return new WaitWhile(() => rb != null && rb.velocity.sqrMagnitude > 0.1f);
        //rb.isKinematic = false;
        //rb.useGravity = true;
        if (navMeshAgent != null)
            navMeshAgent.enabled = true;
        knockbackRoutine = null;
    }

    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    private Coroutine knockbackRoutine;
    private VoicePlayer voicePlayer;
}
