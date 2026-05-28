using Fusion;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcMove : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 10f;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        voicePlayer = GetComponent<VoicePlayer>();
        Debug.Assert(navMeshAgent);
        Debug.Assert(voicePlayer);
        CenterPos = transform.position;
    }

    public bool IsMoving => navMeshAgent.velocity.sqrMagnitude > 0.01f;
    public Vector3 CenterPos { get; set; }

    public bool IsReach()
    {
        return !navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f;
    }

    public void SetRandomDestination()
    {
        NavMeshHit hit;
        Vector3 randomPoint = CenterPos + Random.insideUnitSphere * wanderRadius;
        randomPoint.y = CenterPos.y;

        if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            navMeshAgent.SetDestination(hit.position);
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    public void StopMove()
    {
        navMeshAgent.ResetPath();
    }

    private NavMeshAgent navMeshAgent;
    private VoicePlayer voicePlayer;
}
