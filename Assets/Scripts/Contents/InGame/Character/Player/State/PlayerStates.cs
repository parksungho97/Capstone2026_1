
using UnityEngine;

public class PlayerIdle : State<PlayerContext>
{
    public override void Enter(PlayerContext context)
    {
        // 기본 애니메이션 재생
        context.Animator.SetBool("IsMove", false);
    }

    public override void Exit(PlayerContext context)
    {
    }

    public override void Update(PlayerContext context)
    {
    }
}

public class PlayerWalk : State<PlayerContext>
{
    public override void Enter(PlayerContext context)
    {
        context.Animator.SetBool("IsMove", true);
    }

    public override void Exit(PlayerContext context)
    {
        context.Animator.SetBool("IsMove", false);
        //context.Animator.SetFloat("MoveX", 0f);
        //context.Animator.SetFloat("MoveZ", 0f);
    }

    public override void Update(PlayerContext context)
    {
        Vector3 moveDir = context.Movement.MoveDirection;
        if (moveDir.sqrMagnitude < 0.0001f) return;

        Vector3 viewDir = context.Movement.ViewDirection;
        if (viewDir.sqrMagnitude < 0.0001f)
            viewDir = context.Movement.transform.forward;

        Vector3 localMove = Quaternion.Inverse(Quaternion.LookRotation(viewDir)) * moveDir.normalized;
        // 여기서 다른 애니메이션을 재생하던지 값을 세팅해주던지.
        //context.Animator.SetFloat("MoveX", localMove.x);
        //context.Animator.SetFloat("MoveZ", localMove.z);
    }
}

public class PlayerInteract : State<PlayerContext>
{
    public override void Enter(PlayerContext context)
    {
        context.Animator.SetBool("IsMove", false);
        context.Movement.SetMovePossible(false);
    }

    public override void Exit(PlayerContext context)
    {
        context.Movement.SetMovePossible(true);
    }

    public override void Update(PlayerContext context)
    {
    }
}

public class PlayerAttackState : State<PlayerContext>
{
    public override void Enter(PlayerContext context)
    {
        context.Movement.SetMovePossible(false);
    }

    public override void Exit(PlayerContext context)
    {
        context.Movement.SetMovePossible(true);
    }

    public override void Update(PlayerContext context) { }
}

public class PlayerHit : State<PlayerContext>
{
    public override void Enter(PlayerContext context) { }
    public override void Exit(PlayerContext context) { }
    public override void Update(PlayerContext context) { }
}

public class PlayerReload : State<PlayerContext>
{
    public override void Enter(PlayerContext context) { }
    public override void Exit(PlayerContext context) { }
    public override void Update(PlayerContext context) { }
}

public class PlayerDead : State<PlayerContext>
{
    public override void Enter(PlayerContext context) { }
    public override void Exit(PlayerContext context) { }
    public override void Update(PlayerContext context) { }
}

public class IsWalk : StateTransition
{
    public IsWalk(Movement movement, bool bCompareValue)
    {
        Debug.Assert(movement);
        this.mMovement = movement;
        this.bCompareValue = bCompareValue;
    }
    public override bool ShouldTransition()
    {
        return mMovement.bMove == bCompareValue;
    }

    private Movement mMovement;
    private bool bCompareValue;
}

public class IsInteract : StateTransition
{
    public IsInteract(CapturePointInteracter captureInteracter, bool bCompareValue)
    {
        this.captureInteracter = captureInteracter;
        this.bCompareValue = bCompareValue;
    }
    public override bool ShouldTransition()
    {
        return captureInteracter.IsCapturing == bCompareValue;
    }

    private CapturePointInteracter captureInteracter;
    private bool bCompareValue;
}

public class OnAttackStateTransition : StateTransition
{
    public OnAttackStateTransition(CharacterAttack attacker)
    {
        attacker.ActionAttackStart += () => mPending = true;
    }

    public override bool ShouldTransition()
    {
        if (!mPending) return false;
        mPending = false;
        return true;
    }

    private bool mPending;
}

public class OnAttackEndTransition : StateTransition
{
    public OnAttackEndTransition(CharacterAttack attacker)
    {
        attacker.ActionAttackEnd += () => mPending = true;
    }

    public override bool ShouldTransition()
    {
        if (!mPending) return false;
        mPending = false;
        return true;
    }

    private bool mPending;
}
