using Fusion;
using UnityEngine;

public class PlayerContext
{
    public PlayerContext(Animator animator, PlayerMovement playerMovement)
    {
        Animator = animator;
        PlayerMovement = playerMovement;
    }

    public Animator Animator { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
}

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(PlayerMovement))]
public class PlayerStateManager : MonoBehaviour
{
    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        CapturePointInteracter captureInteractor = GetComponent<CapturePointInteracter>();
        Debug.Assert(animator);
        Debug.Assert(playerMovement);
        Debug.Assert(captureInteractor);

        playerContext = new PlayerContext(animator, playerMovement);

        stateMachine = new StateMachine<PlayerContext>(playerContext);

        playerIdle = new PlayerIdle();
        playerWalk = new PlayerWalk();
        playerInteract = new PlayerInteract();

        isWalk = new IsWalk(playerMovement, true);
        isNotWalk = new IsWalk(playerMovement, false);
        isInteract = new IsInteract(captureInteractor, true);
        isNotInteract = new IsInteract(captureInteractor, false);

        stateMachine.AddTransition(playerIdle, playerWalk, isWalk);
        stateMachine.AddTransition(playerWalk, playerIdle, isNotWalk);
        stateMachine.AddTransition(playerIdle, playerInteract, isInteract);
        stateMachine.AddTransition(playerInteract, playerIdle, isNotInteract);

        stateMachine.SetState(playerIdle);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private PlayerContext playerContext;

    private StateMachine<PlayerContext> stateMachine;

    private PlayerIdle playerIdle;
    private PlayerWalk playerWalk;
    private PlayerInteract playerInteract;

    private IsWalk isWalk;
    private IsWalk isNotWalk;
    private IsInteract isInteract;
    private IsInteract isNotInteract;
}
