using Sandbox;

/// <summary>
/// Is reponsible for managing connections and other high level match details.
/// </summary>
public sealed class MatchManager : SingletonBase<MatchManager>, Component.INetworkListener, IMatchEvents
{
	[Sync] public NetList<Connection> Players { get; private set; } = new();

	[Property] public GameMode GameMode { get; private set; }

	public void StartGame()
	{
		IMatchEvents.Post( e => e.OnGameStart() );
	}

	public void StartGame( GameMode gameMode )
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
