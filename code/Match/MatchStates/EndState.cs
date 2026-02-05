namespace Shooter.Match;

public sealed class EndState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{
    public override void OnEnter()
    {
        matchManager.MatchIsRunning = false;
    }

    public override void OnExit( IState nextState )
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}

