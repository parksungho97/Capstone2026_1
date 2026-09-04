using UnityEngine;

public class PlayerBase : State<PlayerContext>
{
    public override void Enter(PlayerContext context) { }
    public override void Exit(PlayerContext context) { }
    public override void Update(PlayerContext context) { }
}

public class PlayerDontMove : State<PlayerContext>
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

public class PlayerDeadState : State<PlayerContext>
{
    public override void Enter(PlayerContext context) 
    {
        context.PlayerController.bInputDisabled = true;
        context.Movement.SetMovePossible(false);
    }
    public override void Exit(PlayerContext context) 
    {
        context.PlayerController.bInputDisabled = false;
        context.Movement.SetMovePossible(true);
    }
    public override void Update(PlayerContext context) { }
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

public class IsDead : StateTransition
{
    public IsDead(Respawn playerRespawnController, bool bCompareValue)
    {
        this.Respawn = playerRespawnController;
        this.bCompareValue = bCompareValue;
    }

    public override bool ShouldTransition()
    {
        return Respawn.bDead == bCompareValue;
    }

    private Respawn Respawn;
    private bool bCompareValue;
}
