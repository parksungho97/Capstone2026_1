using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationSelector : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NpcAnimationNetworkState animationState;

    [SerializeField] private float crossFadeTime = 0.2f;

    private string currentAnimName = "";
    private bool wasHit = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animationState == null)
            animationState = GetComponent<NpcAnimationNetworkState>();
    }

    private void Update()
    {
        if (animator == null || animationState == null)
            return;

        if (animationState.Object == null || !animationState.Object.IsValid)
            return;

        if (animationState.IsHit && !wasHit)
        {
            CharacterHealth health = GetComponent<CharacterHealth>();

            if (health == null || health.CurrentHP >= 0)
            {
                animator.SetTrigger("Hit");
            }

            animationState.SetHit(false);
        }

        string nextAnimName = animationState.IsMoving
            ? "Pipe_Forward"
            : "Pipe_Idle";

        if (currentAnimName != nextAnimName)
        {
            animator.CrossFade(nextAnimName, crossFadeTime, 0);
            currentAnimName = nextAnimName;
        }

        wasHit = animationState.IsHit;
    }
}