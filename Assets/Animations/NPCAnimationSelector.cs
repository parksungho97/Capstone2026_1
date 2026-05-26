using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcAnimationSelector : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;

    [SerializeField] private float crossFadeTime = 0.2f;
    [SerializeField] private float moveThreshold = 0.05f;

    private string currentAnimName = "";

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (animator == null || navMeshAgent == null)
            return;

        string nextAnimName =
            navMeshAgent.velocity.sqrMagnitude > moveThreshold * moveThreshold
                ? "Pipe_Forward"
                : "Pipe_Idle";

        if (currentAnimName == nextAnimName)
            return;

        animator.CrossFade(nextAnimName, crossFadeTime, 0);
        currentAnimName = nextAnimName;
    }
}