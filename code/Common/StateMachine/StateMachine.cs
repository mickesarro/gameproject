using System.Collections.Generic;

namespace Shooter;

/// <summary>
/// Can be added to classes to make their components state machines.
/// Does not depend on engine specifics.
/// States are added to dictionary and later used through generics.
/// </summary>
public class StateMachine {

    public IState CurrentState { get; private set; }
    public IState PreviousState { get; private set; }

    private readonly Dictionary<System.Type, IState> states = new();

    /// <summary>
    /// Initialize the state machine after having already added the state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void Initialize<T>() where T: IState {
        CurrentState = states[typeof(T)];
        CurrentState?.OnEnter();
    }

    /// <summary>
    /// Initialize the state machine with a new state.
    /// If the new state is not saved yet, save it also.
    /// </summary>
    /// <param name="startingState">IState that gets set and added, if hasn't been already.</param>
    public void Initialize(IState startingState) {
        var state = startingState;
        if (!states.TryAdd(startingState.GetType(), startingState)) {
            state = states[startingState.GetType()];
        }

        CurrentState = state;
        CurrentState.OnEnter();
    }

    /// <summary>
    /// Add a state to the state machine.
    /// </summary>
    /// <param name="state"></param>
    public void AddState(IState state) {
        states.TryAdd(state.GetType(), state);
    }

    public IState GetState<T>() where T: IState {
        return states.GetValueOrDefault(typeof(T));
    }

    public T GetState<T>(bool asRealType) where T : IState
    {
        return (T)states.GetValueOrDefault( typeof( T ) );
    }

    /// <summary>
    /// Calls the current states OnUpdate method.
    /// </summary>
    public void Update() {
		CurrentState?.OnUpdate();
    }

    /// <summary>
    /// Change state to already added state.
    /// Calls OnExit for old and OnEnter for new.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ChangeState<T>() where T: IState {
        PreviousState = CurrentState;
        CurrentState = states[typeof(T)];

        PreviousState?.OnExit(CurrentState);

        CurrentState.OnEnter();
    }

    public void ChangeState(IState nextState) {
        PreviousState = CurrentState;
        CurrentState = nextState;

        PreviousState?.OnExit(CurrentState);

        CurrentState.OnEnter();
    }
}
