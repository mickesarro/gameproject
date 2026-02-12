
using Sandbox;
using System;

namespace Shooter;

internal static class Spawner
{
    public static void SpawnCharacter( PrefabFile prefab, SpawnPoint spawnPoint, PlayerStats prevStats = null )
    {
        // !! This needs to be revamped, we should not need to rely on such ad hoc solution
        //Log.Info( prevStats );
        if ( prefab == null )
        {
            Log.Warning( "[CharacterSpawner] No character prefab set" );
            return;
        }

        // Sometimes fails to fetch earlier
        spawnPoint ??= GetSpawnPoint();

        var character = GameObject.Clone( prefab, transform: spawnPoint.WorldTransform, startEnabled: true );

        if ( character.Components.TryGet<PlayerStats>( out var stats ) )
        {
            stats.Accumulate( prevStats );
        }

        character.NetworkSpawn();
    }

    public static SpawnPoint GetSpawnPoint()
    {
        // This could, and in the future probably should, live in utils
        return Random.Shared.FromArray<GameObject>(
            [.. Game.ActiveScene.GetComponentInChildren<NetworkHelper>().SpawnPoints]
        )?.GetComponent<Shooter.SpawnPoint>();

    }

}
