using Sandbox;
using Shooter.Match;
using System;

namespace Shooter;

/// <summary>
/// Is reponsible for managing connections and other high level match details.
/// </summary>
public sealed class MatchManager : SingletonBase<MatchManager>, Component.INetworkListener, IMatchEvents
{
	[Sync] public NetList<Connection> Players { get; private set; } = new();
    // private int initializedCount = 1;
    [Sync] public int CurrentPlayers { get; private set; } = 0;

    [Sync] public GameMode MatchGameMode { get; private set; }

    private PopulateWithNpcs populator = null;
    [Property] private bool populate = true;

    private StateMachine stateMachine = null;

    // Blocks player movement until everyone is loaded
    public bool MatchIsRunning = false;

    protected override void OnStart()
    {
        base.OnStart();
        //GameObject.NetworkMode = NetworkMode.Object;

        // This might not be the correct place depending on the flow we want
        // e.g. start game only when all players are loaded, or something. 

        //StartGame();
        

        InitialiseFSM();

        if ( Networking.IsHost && populate )
        {
            populator ??= GameObject.GetOrAddComponent<PopulateWithNpcs>( startEnabled: true );
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        stateMachine.Update();
    }

    private void StartGame()
    {
        MatchGameMode = stateMachine.GetState<StartState>(asRealType: true).GameMode;

        IMatchEvents.Post( e => e.OnGameStart() );
	}

	public void StartGame( GameObject gameMode )
	{
		// MatchGameMode = gameMode;
		StartGame();
    }

    private void InitialiseFSM()
    {
        stateMachine ??= new StateMachine();

        IState[] statelist = {
            new StartState(this, stateMachine),
            new MatchState(this, stateMachine),
            new EndState(this, stateMachine)
        };

        foreach (var state in statelist )
        {
            stateMachine.AddState( state );
        }

        stateMachine.Initialize<StartState>();
        
    }

    public void EndGame()
	{
        stateMachine.ChangeState<EndState>();
	}

    /// <summary>
    /// Populates the lobby with NPCs
    /// </summary>
    [Rpc.Host]
    private void TryPopulate()
    {
        if ( CurrentPlayers < MatchGameMode.MaxPlayers ) {
            int npcsToAdd = MatchGameMode.MaxPlayers - CurrentPlayers;
            populator?.SpawnDummys( npcsToAdd );
            CurrentPlayers += npcsToAdd;
        }
    }

    void INetworkListener.OnConnected( Connection channel )
	{
        AddPlayer( channel );
    }

	void INetworkListener.OnDisconnected( Connection channel )
	{
        PlayerCountChanged( false, channel.Id );


        Players.Remove( channel );
        CurrentPlayers--;

        TryPopulate();
    }

    private void AddPlayer( Connection channel )
    {
        Players.Add( channel );
        CurrentPlayers++;
        
        if (CurrentPlayers > MatchGameMode.MaxPlayers) populator?.RemoveDummy();
    }

    void INetworkListener.OnActive( Connection channel )
	{
        PlayerCountChanged( true, Guid.Empty );
        // pitää miettiä, sama ei ehkä toimi dedikoidulla servulla
        if ( channel.IsHost )
        {
            StartGame();
            TryPopulate();
            AddPlayer( channel );
        }
        else
        {
            Log.Info( "setting gamemode: " + GameMode.Current + " on client (t host)" );
            SetCurrentGameMode(GameMode.Current);
        }
    }

    [Rpc.Broadcast( NetFlags.SendImmediate )]
    private void PlayerCountChanged( bool joined, Guid id )
    {
        if ( joined ) {
            IMatchEvents.Post( e => e.OnPlayerJoined() );
        }
        else
        {
            IMatchEvents.Post( e => e.OnPlayerLeft( id ) );
        }
    }

    [Rpc.Broadcast]
    void SetCurrentGameMode(string gamemode)
    {
        if ( GameMode.Current == gamemode )
        {
            Log.Info( "Game mode already set, returning" );
            return;
        }

        GameMode.Current = gamemode;
        
        Log.Info( GameMode.Current );
        
        var clcfg = new CloneConfig
        {
            Parent = GameObject,
            StartEnabled = true,
            Transform = WorldTransform
        };
        
        var mode = GameObject.Clone( GameMode.Current, clcfg );
        
        if ( mode == null || !mode.Components.TryGet<GameMode>( out var gameMode ) )
        {
            Log.Error( "[MatchManager] Passed gameobject prefab does not contain GameMode!" );
            return;
        }
        Log.Info( mode.Name );
        
        // Instantiate the actual gamemode to the scene
        // Should this be network spawned or not?
        //MatchGameMode.Clone( WorldTransform, parent: GameObject );
        MatchGameMode = gameMode;
    }
}
