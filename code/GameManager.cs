namespace Shooter;
public sealed class GameManager : GameObjectSystem<GameManager>, Component.INetworkListener, ISceneStartup
{
	public GameManager( Scene scene ) : base( scene )
	{
	}

	void ISceneStartup.OnClientInitialize()
	{
		// Currently sets up the UI for the client

		var slo = new SceneLoadOptions
		{
			IsAdditive = true
		};
		slo.SetScene( "/scenes/playerScene.scene" );
		Scene.Load( slo );
	}

}
