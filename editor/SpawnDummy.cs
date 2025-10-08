using System;
using System.Linq;
using Sandbox;

using NPC;

/// <summary>
/// Can be used to run scripts in the editor.
/// </summary>
public static class EditorScene
{

	[Event( "scene.play", Priority = 100 )]
	public static void SpawnDummy()
	{
		if ( !Application.IsEditor ) return;

		Log.Info( "[EditorScene.SpawnDummy] Spawned a dummy player." );
		var spawnPoints = Game.ActiveScene.GetAllComponents<SpawnPoint>().ToArray();

		var startLocation = spawnPoints[new Random().Next( 0, spawnPoints.Length )].WorldTransform;

		var NPCGo = GameObject.Clone( "/Dummy.prefab", new CloneConfig { Name = "Dummy", StartEnabled = true, Transform = startLocation } );

		var NPC = NPCGo.GetComponent<NPCController>();

		var waypoints = Game.ActiveScene.FindAllWithTag( "waypoints" );
		NPC.Waypoints = waypoints.First().Children;

		NPC.Initialize( StateEnum.Patrol );

		NPCGo.NetworkSpawn();
	}
}
