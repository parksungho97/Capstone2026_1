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
        Debug.Assert(animator);
        Debug.Assert(playerMovement);

        playerContext = new PlayerContext(animator, playerMovement);

        stateMachine = new StateMachine<PlayerContext>(playerContext);

        playerIdle = new PlayerIdle();
        playerWalk = new PlayerWalk();

        isWalk = new IsWalk(playerMovement, true);
        isNotWalk = new IsWalk(playerMovement, false);

        stateMachine.AddTransition(playerIdle, playerWalk, isWalk);
        stateMachine.AddTransition(playerWalk, playerIdle, isNotWalk);

        stateMachine.SetState(playerIdle);
    }


    private PlayerContext playerContext;

    private StateMachine<PlayerContext> stateMachine;

    private PlayerIdle playerIdle;
    private PlayerWalk playerWalk;

    private IsWalk isWalk;
    private IsWalk isNotWalk;
}
