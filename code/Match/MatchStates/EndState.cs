using Shooter.UI;
using Shooter.UISystem;

namespace Shooter.Match;

public sealed class EndState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{

    TransitionWindow transitionWindow;

    public override StateEnum StateEnum => StateEnum.End;

    public override void OnEnter()
    {
        if ( Networking.IsHost ) {
            matchManager.MatchIsRunning = false;
            matchManager.EndTimerStamp = 0;
            EndTimer = 0;
        }
        EndTimer = matchManager.EndTimerStamp;

        IMatchEvents.Post( e => e.OnGameEnd() );
        
        // Switch to the podium
        matchManager.Scene.GetComponentInChildren<Podium>( includeDisabled: true )
            .GameObject.Enabled = true;

        // Show the win/lose screen
        var HUD = matchManager.Scene.Get<HUD>();
        transitionWindow = HUD.AddComponent<TransitionWindow>( startEnabled: true );

        UIManager.Instance.ShowLayer( transitionWindow, addToHistory: false );

    }

    public override void OnExit( IState nextState )
    {
        return;
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
        if ( Networking.IsHost ) matchManager.EndTimerStamp = EndTimer.Relative;

        if ( EndTimer > statsUITime )
        {
            transitionWindow.Hide();
            UIManager.Instance.ShowLayer<StatsUI>();
            statsUITime = EndTimeLimit * 2; // Makes this if block run only once
        }

        if ( EndTimer < EndTimeLimit ) return;

        stateMachine.ChangeState<VotingState>();
    }
}

