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
    public NpcChaseTarget(NpcMove npcMove, Transform targetTransform)
    {
        this.npcMove = npcMove;
        this.targetTransform = targetTransform;
    }
    public override void Enter(NpcContext npcContext)
    {

    }

    public override void Exit(NpcContext npcContext)
    {
    }

    public override void Update(NpcContext npcContext)
    {
        npcMove.SetDestination(targetTransform.position);
    }

    private NpcMove npcMove;
    private Transform targetTransform;
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
    public MeetTarget(GameObject owner, GameObject target, float detectDistance = 10.0f)
    {
        this.owner = owner;
        this.target = target;
        this.detectDistance = detectDistance;
    }

    public override bool ShouldTransition()
    {
        float dist = (target.transform.position - owner.transform.position).magnitude;
        if (dist < detectDistance)
            return true;
        return false;
    }

    private GameObject owner;
    private GameObject target;
    private float detectDistance;
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
    [SerializeField] private float detectDistance;
    private void Start()
    {
        npcContext = new NpcContext();
        stateMachine = new StateMachine<NpcContext>(npcContext);

        NpcMove npcMove = GetComponent<NpcMove>();
        AudioSource audioSource = GetComponent<AudioSource>();
        Debug.Assert(npcMove);
        Debug.Assert(audioSource);
        Debug.Assert(voiceClipManager);

        idle = new NpcIdle(npcMove);

        stateMachine.SetState(idle);

        npcPlayVoiceFirst = new NpcPlayVoice(voiceClipManager, audioSource);
        npcPlayVoiceSecond = new NpcPlayVoice(voiceClipManager, audioSource);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        stateMachine.Update();
    }

    public void AddTargetChaseState(GameObject target)
    {
        NpcMove npcMove = GetComponent<NpcMove>();

        Debug.Assert(npcMove);

        chaseTarget = new NpcChaseTarget(npcMove, target.transform);
        back = new NpcBack(npcMove, npcMove.CenterPos);

        MeetTarget meetTarget = new MeetTarget(gameObject, target, detectDistance);
        stateMachine.AddTransition(idle, npcPlayVoiceFirst, meetTarget);

        VoiceEnd voiceEnd4 = new VoiceEnd(npcPlayVoiceFirst, 4.0f);
        stateMachine.AddTransition(npcPlayVoiceFirst, npcPlayVoiceSecond, voiceEnd4);

        VoiceEnd voiceEnd0 = new VoiceEnd(npcPlayVoiceSecond, 0.0f);
        stateMachine.AddTransition(npcPlayVoiceSecond, chaseTarget, voiceEnd0);

        TimeOut timeOut = new TimeOut(1.0f);
        stateMachine.AddTransition(chaseTarget, back, timeOut);

        ReachPosition reachPosition = new ReachPosition(gameObject.transform, npcMove.CenterPos);
        stateMachine.AddTransition(back, idle, reachPosition);
    }


    private NpcContext npcContext;
    private StateMachine<NpcContext> stateMachine;

    private NpcIdle idle;
    private NpcChaseTarget chaseTarget;
    private NpcBack back;
    private NpcPlayVoice npcPlayVoiceFirst;
    private NpcPlayVoice npcPlayVoiceSecond;
}