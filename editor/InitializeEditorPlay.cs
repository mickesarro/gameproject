using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Sandbox;
using Shooter.NPC;

namespace Shooter.Editor;

/// <summary>
/// Can be used to run scripts in the editor.
/// </summary>
public static class EditorScene
{

	[Event( "scene.play", Priority = 100 )]
	public static void SpawnDummys()
	{
		if ( !Application.IsEditor || IsMainMenu() ) return;

		Log.Info( "[EditorScene.SpawnDummy] Spawned a dummy player." );

		var spawnPoints = Game.ActiveScene.FindAllWithTag("spawnpoint").ToArray();
		var startLocation = spawnPoints[new Random().Next( 0, spawnPoints.Length )].WorldTransform;

		SpawnDummy( startLocation, StateEnum.Patrol, [StateEnum.Patrol] );

		// Second one
		startLocation = spawnPoints[new Random().Next( 0, spawnPoints.Length )].WorldTransform;

		SpawnDummy( startLocation, StateEnum.Search, [StateEnum.Search, StateEnum.Attack, StateEnum.Hunt] );
	}

	private static void SpawnDummy( Transform startLocation, StateEnum startState, StateEnum[] states )
	{
		var NPCGo = GameObject.Clone( "/Dummy.prefab", new CloneConfig { Name = "Dummy", StartEnabled = true, Transform = startLocation } );

		var NPC = NPCGo.GetComponent<NPCController>();

		NPC.AddStates( states );

		if ( states.Contains( StateEnum.Patrol ) )
		{
			var waypoints = Game.ActiveScene.FindAllWithTag( "waypoints" );
			NPC.Waypoints = waypoints.First().Children;
		}

		//var player = Game.ActiveScene.FindAllWithTag( "player" ).First( e => !e.IsProxy );
		NPC.Initialize( startState );

        NPCGo.NetworkSpawn();
	}

    [Event( "scene.play", Priority = 100 )]
    public static void StartGame()
    {
        if ( Application.IsEditor && !IsMainMenu() )
        {
            var go = GameObject.Clone( "gamemodes/deathmatch.prefab", new CloneConfig { StartEnabled = true } );
            
            var dm = go.GetComponent<GameMode>();

            GameMode.SetGameMode( dm );

            go.Destroy();
        }
    }

    private static bool IsMainMenu()
    {
        return string.Equals( Game.ActiveScene.Name, "mainmenu", StringComparison.OrdinalIgnoreCase );
    }

}
