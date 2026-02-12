using Shooter.UI;
using Shooter.UISystem;

namespace Shooter.Match;

public sealed class EndState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{

    public override void OnEnter()
    {
        matchManager.MatchIsRunning = false;

        IMatchEvents.Post( e => e.OnGameEnd() );
        EndTimer = 0;
        
        // Switch to the podium
        matchManager.Scene.GetComponentInChildren<Podium>( includeDisabled: true )
            .GameObject.Enabled = true;

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
    private readonly float EndTimeLimit = 15.0f;
    private float statsUITime = 7.0f;

    public override void OnUpdate()
    {
        if ( EndTimer > statsUITime )
        {
            UIManager.Instance.ShowLayer<StatsUI>();
            statsUITime = EndTimeLimit * 2; // Makes this if block run only once
        }

        if ( EndTimer < EndTimeLimit ) return;

        OnExit( null );
    }
}

