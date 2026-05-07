using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

[RequireComponent(typeof(NpcMove))]
public class VoiceNPCStateManager : NetworkBehaviour
{
    [SerializeField] private float detectDistance;
    private void Start()
    {
        npcContext = new NpcContext();
        stateMachine = new StateMachine<NpcContext>(npcContext);

        NpcMove npcMove = GetComponent<NpcMove>();
        idle = new NpcIdle(npcMove);

        stateMachine.SetState(idle);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        stateMachine.Update();
    }

    public void AddTargetChaseState(GameObject target)
    {
        NpcMove npcMove = GetComponent<NpcMove>();

        chaseTarget = new NpcChaseTarget(npcMove, target.transform);
        back = new NpcBack(npcMove, npcMove.CenterPos);

        MeetTarget meetTarget = new MeetTarget(gameObject, target, detectDistance);
        stateMachine.AddTransition(idle, chaseTarget, meetTarget);

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
}