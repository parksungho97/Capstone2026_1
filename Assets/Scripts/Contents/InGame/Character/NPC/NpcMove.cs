using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcMove : MonoBehaviour 
{
    [SerializeField] private float wanderRadius = 10f;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        CenterPos = transform.position;
        Debug.Log($"CenterPos: {CenterPos}");
    }

    public bool IsReach()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            return true;
        return false;
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
    public Vector3 CenterPos { get; set; }
    
}
