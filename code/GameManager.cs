using System;

namespace Shooter
    ;
public sealed class GameManager : GameObjectSystem<GameManager>, Component.INetworkListener, ISceneStartup
{
	public GameManager( Scene scene ) : base( scene )
	{
	}

	void ISceneStartup.OnClientInitialize()
	{
        // Currently sets up the UI for the client
        // Game.ActiveScene.Name == "mainmenu"
        if ( string.Equals( Game.ActiveScene.Name, "mainmenu", StringComparison.OrdinalIgnoreCase ) )
		{
			return;
		}

		//LoadUI();
	}

    // Do not use as of now
    public void LoadUI()
	{
		var slo = new SceneLoadOptions
		{
			IsAdditive = true
		};
		slo.SetScene( "/scenes/playerScene.scene_c" );
		Scene.Load( slo );
	}

}
