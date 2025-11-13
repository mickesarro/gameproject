using Sandbox;

namespace Shooter;

/// <summary>
/// Is reponsible for managing connections and other high level match details.
/// </summary>
public sealed class MatchManager : SingletonBase<MatchManager>, Component.INetworkListener, IMatchEvents
{
	[Sync] public NetList<Connection> Players { get; private set; } = new();

	[Property] public GameObject GameMode { get; private set; }

	public void StartGame()
	{
        if ( GameMode == null || !GameMode.Components.TryGet<GameMode>( out _ ) )
        {
            Log.Error( "[MatchManager] Passed gameobject prefab does not contain GameMode!" );
            return;
        }
        
        // Instantiate the actual gamemode to the scene
        // Should this be network spawned or not?
        GameMode.Clone( WorldTransform, parent: GameObject );

        IMatchEvents.Post( e => e.OnGameStart() );
	}

	public void StartGame( GameObject gameMode )
	{
		this.GameMode = gameMode;
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
