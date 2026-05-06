using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTarget : State
{
    public ChaseTarget(NpcMove npcMove, Transform targetTransform)
    {
        this.npcMove = npcMove;
        this.targetTransform = targetTransform;
    }
    public override void Enter()
    {

    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        npcMove.SetDestination(targetTransform.position);
    }

    private NpcMove npcMove;
    private Transform targetTransform;
}

public class Back : State
{
    public Back(NpcMove npcMove, Vector3 originPosition)
    {
        this.npcMove = npcMove;
        this.originPosition = originPosition;
    }
    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
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
        stateMachine = new StateMachine();

        NpcMove npcMove = GetComponent<NpcMove>();
        idle = new Idle(npcMove);

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

        chaseTarget = new ChaseTarget(npcMove, target.transform);
        back = new Back(npcMove, npcMove.CenterPos);

        MeetTarget meetTarget = new MeetTarget(gameObject, target, detectDistance);
        stateMachine.AddTransition(idle, chaseTarget, meetTarget);

        TimeOut timeOut = new TimeOut(1.0f);
        stateMachine.AddTransition(chaseTarget, back, timeOut);

        ReachPosition reachPosition = new ReachPosition(gameObject.transform, npcMove.CenterPos);
        stateMachine.AddTransition(back, idle, reachPosition);
    }
    

    private StateMachine stateMachine;

    private Idle idle;
    private ChaseTarget chaseTarget;
    private Back back;
}