namespace Shooter.Match;

/// <summary>
/// Abstract class for all NPC states to inherit from.
/// Implements the IState interface.
/// </summary>
public abstract class MatchBaseState( MatchManager matchManager, StateMachine stateMachine ) : IState
{
    protected StateMachine stateMachine = stateMachine;
    protected MatchManager matchManager = matchManager;

    public abstract void OnEnter();
    public abstract void OnExit( IState nextState );
    public abstract void OnUpdate();
}
