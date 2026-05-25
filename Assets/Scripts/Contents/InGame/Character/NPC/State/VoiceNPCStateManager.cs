using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcPlayVoice : State<NpcContext>
{
    public NpcPlayVoice(VoiceClipManager voiceClipManager, AudioSource npcAudioSource)
    {
        this.voiceClipManager = voiceClipManager;
        this.npcAudioSource = npcAudioSource;
    }

    public override void Enter(NpcContext context)
    {
        AudioClip clip = voiceClipManager.GetRandomClipOrNull();
        if (clip != null)
        {
            bOriginLooped = npcAudioSource.loop;
            npcAudioSource.loop = false;

            Debug.Log("PlaySound");
            npcAudioSource.PlayOneShot(clip);
        }
    }

    public override void Exit(NpcContext context)
    {
        npcAudioSource.loop = bOriginLooped;
        bSoundEnd = false;
    }

    public override void Update(NpcContext context)
    {
        bSoundEnd = !npcAudioSource.isPlaying;
    }

    private VoiceClipManager voiceClipManager;
    private AudioSource npcAudioSource;

    private bool bOriginLooped = false;

    public bool bSoundEnd { get; private set; }
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
        elapsed += UnityEngine.Time.deltaTime;
        Debug.Log($"{elapsed}");
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

public class VoiceEnd : StateTransition
{
    public VoiceEnd(NpcPlayVoice npcPlayVoice, float duration = 4.0f)
    {
        this.npcPlayVoice = npcPlayVoice;
        this.duration = duration;
    }

    public override bool ShouldTransition()
    {
        if (bWaitState == false)
        {
            if (npcPlayVoice.bSoundEnd)
                bWaitState = true;
        }

        if (bWaitState)
        {
            time += Time.deltaTime;
            if (time >= duration)
            {
                time = 0f;
                bWaitState = false;

                return true;
            }
        }

        return false;
    }

    private NpcPlayVoice npcPlayVoice;

    private bool bWaitState = false;

    private float duration = 0.0f;
    private float time = 0.0f;
}

[RequireComponent(typeof(NpcMove))]
public class VoiceNPCStateManager : NetworkBehaviour
{
    [SerializeField] private VoiceClipManager voiceClipManager;
    [SerializeField] private Chaser chaser;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority == false)
            return;

        npcContext = new NpcContext();
        stateMachine = new StateMachine<NpcContext>(npcContext);

        NpcMove npcMove = GetComponent<NpcMove>();
        AudioSource audioSource = GetComponent<AudioSource>();
        characterHealth = GetComponent<CharacterHealth>();
        Npc npc = GetComponent<Npc>();
        Animator animator = GetComponent<Animator>();

        Debug.Assert(npcMove);
        Debug.Assert(audioSource);
        Debug.Assert(chaser);
        Debug.Assert(voiceClipManager);

        idle = new NpcIdle(npcMove);
        die = new NpcDie(npc, animator);
        npcPlayVoiceFirst = new NpcPlayVoice(voiceClipManager, audioSource);
        npcPlayVoiceSecond = new NpcPlayVoice(voiceClipManager, audioSource);
        chaseTarget = new NpcChaseTarget(npcMove, chaser);
        back = new NpcBack(npcMove, npcMove.CenterPos);

        stateMachine.SetState(idle);
        stateMachine.AddTransition(die, null, new AnimationEnd(animator));
        stateMachine.AddTransition(idle, npcPlayVoiceFirst, new MeetTarget(chaser));
        stateMachine.AddTransition(npcPlayVoiceFirst, npcPlayVoiceSecond, new VoiceEnd(npcPlayVoiceFirst, 4.0f));
        stateMachine.AddTransition(npcPlayVoiceSecond, chaseTarget, new VoiceEnd(npcPlayVoiceSecond, 0.0f));
        stateMachine.AddTransition(chaseTarget, back, new TimeOut(1.0f));
        stateMachine.AddTransition(back, idle, new ReachPosition(gameObject.transform, npcMove.CenterPos));
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority == false)
            return;

        if (!isDead && characterHealth.CurrentHP <= 0)
        {
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
    private NpcBack back;
    private NpcPlayVoice npcPlayVoiceFirst;
    private NpcPlayVoice npcPlayVoiceSecond;
}