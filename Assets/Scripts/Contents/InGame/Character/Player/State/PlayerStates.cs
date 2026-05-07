
using System.Diagnostics;

public class PlayerIdle : State<PlayerContext>
{
    public override void Enter(PlayerContext context)
    {
        // 기본 애니메이션 재생
        
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
    }

    public override void Update(PlayerContext context)
    {
    }
}

public class IsWalk : StateTransition
{
    public IsWalk(PlayerMovement playerMovement, bool bCompareValue)
    {
        Debug.Assert(playerMovement);
        this.playerMovement = playerMovement;
        this.bCompareValue = bCompareValue;
    }
    public override bool ShouldTransition()
    {
        return playerMovement.bMove == bCompareValue;
    }

    private PlayerMovement playerMovement;
    private bool bCompareValue;
}