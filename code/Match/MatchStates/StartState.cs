
namespace Shooter.Match;

public sealed class StartState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine )
{
    public GameMode GameMode { get; private set; }

    public override void OnEnter()
    {
        if ( !Networking.IsHost ) return;

        var clcfg = new CloneConfig
        {
            Parent = matchManager.GameObject,
            StartEnabled = true,
            Transform = matchManager.WorldTransform
        };

        var mode = GameObject.Clone( GameMode.Current, clcfg );

        if ( mode == null || !mode.Components.TryGet<GameMode>( out var gameMode ) )
        {
            Log.Error( "[MatchManager] Passed gameobject prefab does not contain GameMode!" );
            return;
        }
        mode.NetworkSpawn();
        Log.Info( mode.Name );

        // Instantiate the actual gamemode to the scene
        // Should this be network spawned or not?
        //MatchGameMode.Clone( WorldTransform, parent: GameObject );
        GameMode = gameMode;

        IMatchEvents.Post( e => e.OnGameStart() );
    }

    public override void OnExit( IState nextState )
    {
        // Clear it as it is not needed here anymore
        GameMode = null;
    }

    public override void OnUpdate()
    {
        if ( !Networking.IsHost ) return;
        else if ( matchManager.GoToNextState )
        {
            stateMachine.ChangeState<MatchState>();
        }

        foreach ( var con in Connection.All )
        {
            if ( !con.IsActive ) return;
        }

        // Add some player spawn timer thing here, seperate state or match

        matchManager.GoToNextState = true;
        stateMachine.ChangeState<MatchState>();

    }

}

