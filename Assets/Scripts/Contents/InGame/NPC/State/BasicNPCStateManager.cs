using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : State
{
    public Idle(NpcMove npcMove)
    {
        this.npcMove = npcMove;
        Debug.Assert(npcMove);
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (npcMove.IsReach())
            npcMove.SetRandomDestination();
    }

    private NpcMove npcMove;
}

public class BasicNPCStateManager : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();

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

    private StateMachine stateMachine;

    private Idle idle;
}
