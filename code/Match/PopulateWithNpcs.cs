using Shooter.NPC;
using System;

namespace Shooter;

/// <summary>
/// Used for spawning NPCs in game.
/// </summary>
public sealed class PopulateWithNpcs : Component
{
    // This is merely an ad-hoc solution for demo before we develop a better populating
    // logic based on lobby size and real player count etc.

    public void SpawnDummys( int amount = 4 ) // Non-static as we don't want anyone to do this
    {

        var NPCGo = GameObject.Clone( "/Dummy.prefab", new CloneConfig { Name = "Dummy", StartEnabled = true } );

        var NPC = NPCGo.GetComponent<NPCController>();

        NPC.AddStates( [StateEnum.Search, StateEnum.Attack, StateEnum.Hunt] );
        NPC.Initialize( StateEnum.Search );

        for ( int i = 0; i < amount; ++i )
        {
            Spawner.SpawnCharacter( NPCGo, name: "Dummy" );
        }

        NPCGo.DestroyImmediate();

        Log.Info( $"[PopulateWithNpcs.SpawnDummys] Spawned {amount} dummy players." );
    }

    [Obsolete("Use Spawner class")]
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
