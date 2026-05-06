using System.Collections.Generic;

public abstract class State
{
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

public abstract class StateTransition
{
    public abstract bool ShouldTransition();
}

public class StateMachine
{
    public void AddTransition(State from, State to, StateTransition transition)
    {
        if (!transitions.ContainsKey(from))
            transitions[from] = new List<(StateTransition, State)>();
        transitions[from].Add((transition, to));
    }

    public void SetState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();

        if (!transitions.ContainsKey(currentState))
            transitions[currentState] = new List<(StateTransition, State)>();
        currentTransitions = transitions[currentState];
    }

    public void Update()
    {
        if (currentState == null) return;

        currentState.Update();

        foreach (var (transition, targetState) in currentTransitions)
        {
            if (transition.ShouldTransition())
            {
                SetState(targetState);
                break;
            }
        }
    }

    private State currentState;
    private List<(StateTransition, State)> currentTransitions;
    private Dictionary<State, List<(StateTransition, State)>> transitions = new();
}