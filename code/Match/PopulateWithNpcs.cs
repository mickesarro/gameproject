using Shooter.NPC;
using System;
using System.Collections.Generic;

namespace Shooter;

/// <summary>
/// Used for spawning NPCs in game.
/// </summary>
public sealed class PopulateWithNpcs : Component
{
    // This is merely an ad-hoc solution for demo before we develop a better populating
    // logic based on lobby size and real player count etc.

    // private HashSet<GameObject> npcs = new();

    public void SpawnDummys(int amount = 4) // Non-static as we don't want anyone to do this
    {
        var spawnPoints = Game.ActiveScene.FindAllWithTag( "spawnpoint" ).ToArray();

        for ( int i = 0; i < amount; ++i )
        {
            var startLocation = spawnPoints[new Random().Next( 0, spawnPoints.Length )].WorldTransform;

            SpawnDummy( startLocation, StateEnum.Search, [StateEnum.Search, StateEnum.Attack, StateEnum.Hunt] );
        }

        Log.Info( $"[PopulateWithNpcs.SpawnDummys] Spawned {amount} dummy players." );
    }

    public GameObject SpawnDummy( Transform startLocation, StateEnum startState, StateEnum[] states )
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

        return NPCGo;
    }

    public void RemoveDummy()
    {
        Scene.FindAllWithTag( "npc" ).FirstOrDefault().Destroy();
    }
}
