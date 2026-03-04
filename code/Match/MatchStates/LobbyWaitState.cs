
namespace Shooter.Match;

public sealed class LobbyWaitState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{
    public override StateEnum StateEnum => StateEnum.LobbyWait;

    private RealTimeSince LobbyWait;
    private float MaxLobbyWaitTime = 10f;

    public override void OnEnter()
    {
        IMatchEvents.Post( e => e.OnWaitPlayers() );

        if ( !Networking.IsHost ) return;

#if DEBUG
        HoldLobby = true;
#endif

    }

    public override void OnExit( IState nextState )
    {
        IMatchEvents.Post( e => e.OnGameStart() );
    }

    [ConVar(
        Help = "Hold the lobby from starting",
        Flags = ConVarFlags.Admin | ConVarFlags.Hidden | ConVarFlags.Protected
    ), SkipHotload]
    public static bool HoldLobby { get; set; } = false;

    public override void OnUpdate()
    {
        if ( matchManager.GoToNextState )
        {
            stateMachine.ChangeState<MatchState>();
        }
        else if ( !Networking.IsHost || HoldLobby ) return;

        // Start the game regardless if max wait time too much
        if ( LobbyWait > MaxLobbyWaitTime )
        {
            StartMatch();
            return;
        }

        var connections = Connection.All;

        // If less than min players, continue waiting
        if ( matchManager.MatchGameMode.MinPlayers > connections.Count ) return;

        foreach ( var con in connections )
        {
            if ( !con.IsActive ) return;
        }

        // Add some player spawn timer thing here, seperate state or match

        StartMatch();

    }

    private void StartMatch()
    {
        matchManager.GoToNextState = true;
        stateMachine.ChangeState<MatchState>();
    }

}

