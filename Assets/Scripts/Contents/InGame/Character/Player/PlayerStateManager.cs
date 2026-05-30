using Fusion;
using UnityEngine;

public class PlayerContext
{
    public PlayerContext(Animator animator, Movement movement, VoicePlayer voicePlayer
        , PlayerController playerController, Respawn respawn)
    {
        Animator = animator;
        Movement = movement;
        VoicePlayer = voicePlayer;
        PlayerController = playerController;
        Respawn = respawn;
    }

    public Animator Animator { get; private set; }
    public Movement Movement { get; private set; }
    public VoicePlayer VoicePlayer { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public Respawn Respawn { get; private set; }
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
        VoicePlayer voicePlayer = GetComponent<VoicePlayer>();
        PlayerController playerController = GetComponent<PlayerController>();
        Respawn respawn = GetComponent<Respawn>();

        Debug.Assert(animator);
        Debug.Assert(movement);
        Debug.Assert(captureInteractor);
        Debug.Assert(voicePlayer);
        Debug.Assert(playerController);
        Debug.Assert(respawn);

        playerContext = new PlayerContext(animator, movement, voicePlayer, playerController, respawn);
        stateMachine = new StateMachine<PlayerContext>(playerContext);

        playerBase = new PlayerBase();
        playerDontMove = new PlayerDontMove();
        playerDead = new PlayerDeadState();

        isInteract = new IsInteract(captureInteractor, true);
        isNotInteract = new IsInteract(captureInteractor, false);
        isDead = new IsDead(respawn, true);
        isNotDead = new IsDead(respawn, false);

        stateMachine.AddTransition(playerBase, playerDontMove, isInteract);
        stateMachine.AddTransition(playerDontMove, playerBase, isNotInteract);
        stateMachine.AddTransition(playerBase, playerDead, isDead);
        stateMachine.AddTransition(playerDontMove, playerDead, isDead);
        stateMachine.AddTransition(playerDead, playerBase, isNotDead);

        stateMachine.SetState(playerBase);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private PlayerContext playerContext;
    private StateMachine<PlayerContext> stateMachine;

    private PlayerBase playerBase;
    private PlayerDontMove playerDontMove;
    private PlayerDeadState playerDead;

    private IsInteract isInteract;
    private IsInteract isNotInteract;
    private IsDead isDead;
    private IsDead isNotDead;
}
