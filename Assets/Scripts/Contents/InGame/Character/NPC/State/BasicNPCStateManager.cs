using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcContext
{
    
}
public class NpcIdle : State<NpcContext>
{
    public NpcIdle(NpcMove npcMove)
    {
        this.npcMove = npcMove;
        Debug.Assert(npcMove);
    }

    public override void Enter(NpcContext npcContext)
    {
    }

    public override void Exit(NpcContext npcContext)
    {
    }

    public override void Update(NpcContext npcContext)
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

    private NpcContext npcContext;
    private StateMachine<NpcContext> stateMachine;

    private NpcIdle idle;
}
