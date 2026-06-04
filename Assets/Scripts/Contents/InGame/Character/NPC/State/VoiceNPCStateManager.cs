using Fusion;
using UnityEngine;

public class NpcPlayVoice : State<NpcContext>
{
    public NpcPlayVoice(VoicePlayer voicePlayer, NpcMove npcMove)
    {
        this.voicePlayer = voicePlayer;
        this.npcMove = npcMove;
    }

    public override void Enter(NpcContext context)
    {
        npcMove.StopMove();
        voicePlayer.PlayRandomOnAll();
    }

    public override void Exit(NpcContext context) { }

    public override void Update(NpcContext context) { }

    private VoicePlayer voicePlayer;
    private NpcMove npcMove;
}
public class NpcChaseTarget : State<NpcContext>
{
    public NpcChaseTarget(NpcMove npcMove, Chaser chaser)
    {
        this.npcMove = npcMove;
        this.chaser = chaser;
    }

    public override void Enter(NpcContext npcContext) { }

    public override void Exit(NpcContext npcContext) { }

    public override void Update(NpcContext npcContext)
    {
        Transform target = chaser.NearestTarget;
        if (target != null)
            npcMove.SetDestination(target.position);
    }

    private NpcMove npcMove;
    private Chaser chaser;
}

public class NpcAttack : State<NpcContext>
{
    public NpcAttack(Transform self, NpcMove npcMove, NpcAttackNetworkState npcAttackNetworkState, Chaser chaser)
    {
        this.self = self;
        this.npcMove = npcMove;
        this.npcAttackNetworkState = npcAttackNetworkState;
        this.chaser = chaser;
    }

    public override void Enter(NpcContext context)
    {
        npcMove.StopMove();
    }

    public override void Update(NpcContext context)
    {
        Transform target = chaser.NearestTarget;
        if (target == null) return;

        Vector3 dir = target.position - self.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            self.rotation = Quaternion.LookRotation(dir);

        npcAttackNetworkState.Attack();
    }

    public override void Exit(NpcContext context) { }

    private readonly Transform self;
    private readonly NpcMove npcMove;
    private readonly NpcAttackNetworkState npcAttackNetworkState;
    private readonly Chaser chaser;
}

public class WithinAttackRange : StateTransition
{
    public WithinAttackRange(Chaser chaser, Transform self, float range)
    {
        this.chaser = chaser;
        this.self = self;
        sqrRange = range * range;
    }

    public override bool ShouldTransition()
    {
        Transform target = chaser.NearestTarget;
        if (target == null) return false;
        return (target.position - self.position).sqrMagnitude <= sqrRange;
    }

    private readonly Chaser chaser;
    private readonly Transform self;
    private readonly float sqrRange;
}

public class OutOfAttackRange : StateTransition
{
    public OutOfAttackRange(Chaser chaser, Transform self, float range)
    {
        this.chaser = chaser;
        this.self = self;
        sqrRange = range * range;
    }

    public override bool ShouldTransition()
    {
        Transform target = chaser.NearestTarget;
        if (target == null) return true;
        return (target.position - self.position).sqrMagnitude > sqrRange;
    }

    private readonly Chaser chaser;
    private readonly Transform self;
    private readonly float sqrRange;
}

public class NpcBack : State<NpcContext>
{
    public NpcBack(NpcMove npcMove, Vector3 originPosition)
    {
        this.npcMove = npcMove;
        this.originPosition = originPosition;
    }
    public override void Enter(NpcContext npcContext)
    {
    }

    public override void Exit(NpcContext npcContext)
    {
    }

    public override void Update(NpcContext npcContext)
    {
        npcMove.SetDestination(originPosition);
    }

    private NpcMove npcMove;
    private Vector3 originPosition;
}

public class MeetTarget : StateTransition
{
    public MeetTarget(Chaser chaser)
    {
        this.chaser = chaser;
    }

    public override bool ShouldTransition() => chaser.HasTarget;

    private Chaser chaser;
}

public class TimeOut : StateTransition
{
    public TimeOut(float duration)
    {
        this.duration = duration;
    }

    public override bool ShouldTransition()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= duration)
        {
            elapsed = 0f;
            return true;
        }
        return false;
    }

    private float duration;
    private float elapsed;
}

public class ReachPosition : StateTransition
{
    public ReachPosition(Transform transform, Vector3 originPosition)
    {
        this.transform = transform;
        this.originPosition = originPosition;
    }

    public override bool ShouldTransition()
    {
        if ((transform.position - originPosition).magnitude < 2.0f)
            return true;
        return false;
    }

    private Transform transform;
    private Vector3 originPosition;
}

// Fires when the voice clip finishes (plus an optional delay) AND the target condition is met.
// requiresTarget=true  → transitions only if a target is nearby
// requiresTarget=false → transitions only if no target is nearby
public class VoiceEndConditional : StateTransition
{
    public VoiceEndConditional(VoicePlayer voicePlayer, Chaser chaser, float duration, bool requiresTarget)
    {
        this.voicePlayer = voicePlayer;
        this.chaser = chaser;
        this.duration = duration;
        this.requiresTarget = requiresTarget;
    }

    public override void Reset()
    {
        bWaitState = false;
        time = 0f;
    }

    public override bool ShouldTransition()
    {
        if (!bWaitState && !voicePlayer.IsPlaying)
            bWaitState = true;

        if (!bWaitState) return false;

        time += Time.deltaTime;
        if (time < duration) return false;

        return requiresTarget ? chaser.HasTarget : !chaser.HasTarget;
    }

    private readonly VoicePlayer voicePlayer;
    private readonly Chaser chaser;
    private readonly float duration;
    private readonly bool requiresTarget;

    private bool bWaitState;
    private float time;
}

[RequireComponent(typeof(NpcMove))]
public class VoiceNPCStateManager : NetworkBehaviour
{
    [SerializeField] private Chaser chaser;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float chaseTime = 3.0f;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority == false)
            return;

        npcContext = new NpcContext();
        stateMachine = new StateMachine<NpcContext>(npcContext);

        NpcMove npcMove = GetComponent<NpcMove>();
        VoicePlayer voicePlayer = GetComponent<VoicePlayer>();
        characterHealth = GetComponent<CharacterHealth>();
        NpcAttackNetworkState npcAttackNetworkState = GetComponent<NpcAttackNetworkState>();
        Npc npc = GetComponent<Npc>();
        Animator animator = GetComponentInChildren<Animator>();

        Debug.Assert(npcMove);
        Debug.Assert(voicePlayer);
        Debug.Assert(chaser);
        Debug.Assert(npcAttackNetworkState);

        idle = new NpcIdle(npcMove);
        die = new NpcDie(npc, animator);
        npcPlayVoiceFirst = new NpcPlayVoice(voicePlayer, npcMove);
        npcPlayVoiceSecond = new NpcPlayVoice(voicePlayer, npcMove);
        chaseTarget = new NpcChaseTarget(npcMove, chaser);
        npcAttack = new NpcAttack(transform, npcMove, npcAttackNetworkState, chaser);
        back = new NpcBack(npcMove, npcMove.CenterPos);

        stateMachine.AddTransition(idle, npcPlayVoiceFirst, new MeetTarget(chaser));

        stateMachine.AddTransition(npcPlayVoiceFirst, npcPlayVoiceSecond, new VoiceEndConditional(voicePlayer, chaser, 4.0f, requiresTarget: true));
        stateMachine.AddTransition(npcPlayVoiceFirst, idle,              new VoiceEndConditional(voicePlayer, chaser, 4.0f, requiresTarget: false));

        stateMachine.AddTransition(npcPlayVoiceSecond, chaseTarget, new VoiceEndConditional(voicePlayer, chaser, 0.0f, requiresTarget: true));
        stateMachine.AddTransition(npcPlayVoiceSecond, idle,         new VoiceEndConditional(voicePlayer, chaser, 0.0f, requiresTarget: false));

        stateMachine.AddTransition(chaseTarget, npcAttack, new WithinAttackRange(chaser, transform, attackRange));
        stateMachine.AddTransition(chaseTarget, back, new TimeOut(chaseTime));

        stateMachine.AddTransition(npcAttack, chaseTarget, new OutOfAttackRange(chaser, transform, attackRange));
        stateMachine.AddTransition(back, idle, new ReachPosition(gameObject.transform, npcMove.CenterPos));
        stateMachine.AddTransition(die, null, new AnimationEnd(animator, "Npc_Dead"));

        stateMachine.SetState(idle);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority == false)
            return;
        //Debug.Log(stateMachine.CurrentState.GetType());
        if (!isDead && characterHealth.CurrentHP <= 0)
        {
            Debug.Log("Dead");
            isDead = true;
            stateMachine.SetState(die);
            return;
        }

        stateMachine.Update();
    }

    private NpcContext npcContext;
    private StateMachine<NpcContext> stateMachine;
    private CharacterHealth characterHealth;
    private bool isDead;

    private NpcIdle idle;
    private NpcDie die;
    private NpcChaseTarget chaseTarget;
    private NpcAttack npcAttack;
    private NpcBack back;
    private NpcPlayVoice npcPlayVoiceFirst;
    private NpcPlayVoice npcPlayVoiceSecond;
}
