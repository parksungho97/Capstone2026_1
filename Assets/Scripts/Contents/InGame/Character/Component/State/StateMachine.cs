using System.Collections.Generic;

public abstract class State<T>
{
    public abstract void Enter(T context);
    public abstract void Update(T context);
    public abstract void Exit(T context);
}

public abstract class StateTransition
{
    public abstract bool ShouldTransition();
}

public class StateMachine<T>
{
    public StateMachine(T context)
    {
        this.context = context;
    }
    public void AddTransition(State<T> from, State<T> to, StateTransition transition)
    {
        if (!transitions.ContainsKey(from))
            transitions[from] = new List<(StateTransition, State<T>)>();
        transitions[from].Add((transition, to));
    }

    public void SetState(State<T> newState)
    {
        currentState?.Exit(context);
        currentState = newState;
        currentState?.Enter(context);

        if (currentState == null)
        {
            currentTransitions = new List<(StateTransition, State<T>)>();
            return;
        }

        if (!transitions.ContainsKey(currentState))
            transitions[currentState] = new List<(StateTransition, State<T>)>();
        currentTransitions = transitions[currentState];
    }

    public void Update()
    {
        if (currentState == null) return;

        currentState.Update(context);

        foreach (var (transition, targetState) in currentTransitions)
        {
            if (transition.ShouldTransition())
            {
                SetState(targetState);
                break;
            }
        }
    }

    private T context;
    private State<T> currentState;
    private List<(StateTransition, State<T>)> currentTransitions;
    private Dictionary<State<T>, List<(StateTransition, State<T>)>> transitions = new();
}