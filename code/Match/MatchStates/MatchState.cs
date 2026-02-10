using Shooter.UISystem;
using System.Threading.Tasks;
using Shooter.UI;
using Shooter.Sounds;

namespace Shooter.Match;

public sealed class MatchState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{
    public int StartTimer { get; set; } = 0;

    private HUDTimer HUDTimer = null;

    public override void OnEnter()
    {
        StartTimer = matchManager.MatchGameMode.StartCountdown;

        // Add the timer runtime to HUD to manage lifetime well
        var HUD = matchManager.Scene.Get<HUD>();
        HUDTimer = HUD.AddComponent<HUDTimer>( true );

        UIManager.Instance.ShowLayerWithData( HUDTimer, this, addToHistory: false );

        _ = Timer();

        if ( Networking.IsHost )
        {
            matchManager.GoToNextState = false;
        }
    }

    public override void OnExit( IState nextState )
    {
        return;
    }

    public override void OnUpdate()
    {
        if ( !matchManager.MatchIsRunning ) return;
    }

    /// <summary>
    /// Is responsible for handling the game start countdown
    /// </summary>
    /// <returns></returns>
    private async Task Timer()
    {
        // A few notes on implementation:
        // A utility such as this should not handle audio as well

        while ( StartTimer > 0 )
        {
            // Add specific match start sounds later
            SoundManager.PlayLocal( SoundManager.SoundType.UIAccept );
            await GameTask.DelaySeconds( 1.0f );
            StartTimer--;
        }

        matchManager.MatchIsRunning = true;
        UIManager.Instance.ResetToStartLayer();
        HUDTimer.Destroy();

        SoundManager.PlayLocal( SoundManager.SoundType.UIAccept, 3 );
    }

}

