
public sealed class GameManager : GameObjectSystem<GameManager>, Component.INetworkListener, ISceneStartup
{
	public GameManager( Scene scene ) : base( scene )
	{
	}

	void ISceneStartup.OnClientInitialize()
	{
		// Currently sets up the UI for the client
		if ( Game.ActiveScene.Name == "mainmenu" )
		{
			return;
		}

		LoadUI();
	}

	public void LoadUI()
	{
		var slo = new SceneLoadOptions
		{
			IsAdditive = true
		};
		slo.SetScene( "/scenes/playerScene.scene" );
		Scene.Load( slo );
	}

}
