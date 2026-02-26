using Shooter.UISystem;
using System.Threading.Tasks;
using Shooter.UI;
using Shooter.Sounds;

namespace Shooter.Match;

public sealed class MatchState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine ), ICountdownable
{
    public int StartTimer { get; set; } = 0;

    int ICountdownable.GetTime() => StartTimer;
    private bool countdownActive = false;
    bool ICountdownable.IsActive => countdownActive;
    bool ICountdownable.Skippable => false;

    public override void OnEnter()
    {
        StartTimer = matchManager.MatchGameMode.StartCountdown;

        IMatchEvents.Post( e => e.OnCountdownStart( this ) );

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

        countdownActive = true;

        while ( StartTimer > 0 )
        {
            // Add specific match start sounds later
            SoundManager.PlayLocal( SoundManager.SoundType.UIAccept );
            await GameTask.DelaySeconds( 1.0f );
            StartTimer--;
        }

        matchManager.MatchIsRunning = true;
        countdownActive = false;

        SoundManager.PlayLocal( SoundManager.SoundType.UIAccept, 3 );
    }

}

