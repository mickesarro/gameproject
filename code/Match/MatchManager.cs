using Sandbox;
using static Sandbox.PhysicsGroupDescription.BodyPart;

namespace Shooter;

/// <summary>
/// Is reponsible for managing connections and other high level match details.
/// </summary>
public sealed class MatchManager : SingletonBase<MatchManager>, Component.INetworkListener, IMatchEvents
{
	[Sync] public NetList<Connection> Players { get; private set; } = new();

	public GameObject MatchGameMode { get; private set; }

    protected override void OnStart()
    {
        base.OnStart();

        // This might not be the correct place depending on the flow we want
        // e.g. start game only when all players are loaded, or something. 
        StartGame();
    }

    public void StartGame()
    {
        var clcfg = new CloneConfig
        {
            Parent = GameObject,
            StartEnabled = true,
            Transform = WorldTransform
        };

        var mode = GameObject.Clone( GameMode.Current, clcfg );

        if ( mode == null || !mode.Components.TryGet<GameMode>( out _ ) )
        {
            Log.Error( "[MatchManager] Passed gameobject prefab does not contain GameMode!" );
            return;
        }
        Log.Info( mode.Name );

        // Instantiate the actual gamemode to the scene
        // Should this be network spawned or not?
        //MatchGameMode.Clone( WorldTransform, parent: GameObject );
        MatchGameMode = mode;

        IMatchEvents.Post( e => e.OnGameStart() );
	}

	public void StartGame( GameObject gameMode )
	{
		MatchGameMode = gameMode;
		StartGame();
	}

	public void EndGame()
	{
		IMatchEvents.Post( e => e.OnGameEnd() );
	}

	void INetworkListener.OnConnected( Connection channel )
	{
		Players.Add( channel );
	}

	void INetworkListener.OnDisconnected( Connection channel )
	{
		IMatchEvents.Post( e => e.OnPlayerLeft() );

		Players.Remove( channel );
	}

	void INetworkListener.OnActive( Connection channel )
	{
		IMatchEvents.Post( e => e.OnPlayerJoined() );
	}

}
