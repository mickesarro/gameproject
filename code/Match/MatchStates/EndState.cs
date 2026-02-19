using Shooter.UI;
using Shooter.UISystem;

namespace Shooter.Match;

public sealed class EndState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{
    public override void OnEnter()
    {
        matchManager.MatchIsRunning = false;

        UIManager.Instance.ShowLayer<StatsUI>();

        IMatchEvents.Post( e => e.OnGameEnd() );
        EndTimer = 0;
    }

    public override void OnExit( IState nextState )
    {
        // Implement scene manager
        Log.Info( "Main menu" );

        Networking.Disconnect();

        // Once again, make a scene manager
        // Save stats etc. cleanup
        var slo = new SceneLoadOptions { };
        slo.SetScene( "/scenes/mainMenu.scene" );
        //matchManager.Scene.Clear();
        matchManager.Scene.Load( slo );
        
    }

    private TimeSince EndTimer;
    private float EndTimeLimit = 10.0f;

    public override void OnUpdate()
    {
        while ( EndTimer < EndTimeLimit ) return;
        OnExit( null );
    }
}

