namespace Shooter.Match;

public sealed class MatchState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{
    public override void OnEnter()
    {
        return;
    }

    public override void OnExit( IState nextState )
    {
        return;
    }

    public override void OnUpdate()
    {
        
    }
}

