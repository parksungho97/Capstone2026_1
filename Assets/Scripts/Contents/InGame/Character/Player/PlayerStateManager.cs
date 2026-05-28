using Fusion;
using UnityEngine;

public class PlayerContext
{
    public PlayerContext(Animator animator, Movement movement, VoicePlayer voicePlayer, PlayerController playerController)
    {
        Animator = animator;
        Movement = movement;
        VoicePlayer = voicePlayer;
        PlayerController = playerController;
    }

    public Animator Animator { get; private set; }
    public Movement Movement { get; private set; }

    public VoicePlayer VoicePlayer { get; private set; }

    public PlayerController PlayerController { get; private set;  }
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Movement))]
public class PlayerStateManager : MonoBehaviour
{
    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        Movement movement = GetComponent<Movement>();
        CapturePointInteracter captureInteractor = GetComponent<CapturePointInteracter>();
        CharacterAttack characterAttack = GetComponent<CharacterAttack>();
        VoicePlayer voicePlayer = GetComponent<VoicePlayer>();
        PlayerController playerController = GetComponent<PlayerController>();
        Debug.Assert(animator);
        Debug.Assert(movement);
        Debug.Assert(captureInteractor);
        Debug.Assert(characterAttack);
        Debug.Assert(voicePlayer);
        Debug.Assert(playerController);

        playerContext = new PlayerContext(animator, movement, voicePlayer, playerController);

        stateMachine = new StateMachine<PlayerContext>(playerContext);

        playerIdle = new PlayerIdle();
        playerWalk = new PlayerWalk();
        playerInteract = new PlayerInteract();
        playerAttackState = new PlayerAttackState();

        isWalk = new IsWalk(playerController, true);
        isNotWalk = new IsWalk(playerController, false);
        isInteract = new IsInteract(captureInteractor, true);
        isNotInteract = new IsInteract(captureInteractor, false);
        onAttack = new OnAttackStateTransition(characterAttack);
        onAttackEnd = new OnAttackEndTransition(characterAttack);

        stateMachine.AddTransition(playerIdle, playerWalk, isWalk);
        stateMachine.AddTransition(playerWalk, playerIdle, isNotWalk);
        stateMachine.AddTransition(playerIdle, playerInteract, isInteract);
        stateMachine.AddTransition(playerInteract, playerIdle, isNotInteract);
        stateMachine.AddTransition(playerIdle, playerAttackState, onAttack);
        stateMachine.AddTransition(playerWalk, playerAttackState, onAttack);
        stateMachine.AddTransition(playerAttackState, playerIdle, onAttackEnd);

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
    private PlayerAttackState playerAttackState;

    private IsWalk isWalk;
    private IsWalk isNotWalk;
    private IsInteract isInteract;
    private IsInteract isNotInteract;
    private OnAttackStateTransition onAttack;
    private OnAttackEndTransition onAttackEnd;
}
