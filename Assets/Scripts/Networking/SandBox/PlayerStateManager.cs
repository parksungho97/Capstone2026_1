using Fusion;
using UnityEngine;

public class PlayerStateManager : NetworkBehaviour
{
    private void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator);
    }

    [Networked, OnChangedRender(nameof(OnMoveChanged))]
    public bool Move { get; set; }

    private void OnMoveChanged()
    {
        animator.SetBool("IsMove", Move);
    }

    private Animator animator;
}
