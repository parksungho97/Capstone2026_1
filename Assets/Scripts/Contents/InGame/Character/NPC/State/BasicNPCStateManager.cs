using UnityEngine;

public class NpcContext
{

}

public class NpcIdle : State<NpcContext>
{
    public NpcIdle(NpcMove npcMove)
    {
        this.npcMove = npcMove;
    }

    public override void Enter(NpcContext npcContext)
    {
        npcMove.SetRandomDestination();
    }

    public override void Exit(NpcContext npcContext)
    {
        npcMove.StopMove();
    }

    public override void Update(NpcContext npcContext)
    {
        if (npcMove.IsReach())
            npcMove.SetRandomDestination();
    }
    private NpcMove npcMove;
}

public class NpcDie : State<NpcContext>
{
    public NpcDie(Npc npc, Animator animator)
    {
        this.npc = npc;
        this.animator = animator;
    }

    public override void Enter(NpcContext context)
    {
        //if (animator)
        //    animator.SetTrigger("Die");
    }

    public override void Update(NpcContext context) { }

    public override void Exit(NpcContext context)
    {
        npc.DestroyNpc();
    }

    private Npc npc;
    private Animator animator;
}

public class AnimationEnd : StateTransition
{
    public AnimationEnd(Animator animator)
    {
        this.animator = animator;
    }

    public override bool ShouldTransition()
    {
        if (animator == null)
            return true;
        if (animator.IsInTransition(0)) return false;
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }

    private Animator animator;
}

public class BasicNPCStateManager : MonoBehaviour
{
    private void Start()
    {
        npcContext = new NpcContext();
        stateMachine = new StateMachine<NpcContext>(npcContext);

        NpcMove npcMove = GetComponent<NpcMove>();
        characterHealth = GetComponent<CharacterHealth>();
        Npc npc = GetComponent<Npc>();
        Animator animator = GetComponent<Animator>();

        idle = new NpcIdle(npcMove);
        die = new NpcDie(npc, animator);

        stateMachine.SetState(idle);
        stateMachine.AddTransition(die, null, new AnimationEnd(animator));
    }

    private void Update()
    {
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
}
