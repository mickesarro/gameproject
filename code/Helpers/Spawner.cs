
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

    /// <summary>
    /// Get a spawn point from the map.
    /// </summary>
    /// <param name="checkForOthers">Determines whether to check nearby characters.</param>
    /// <returns></returns>
    public static SpawnPoint GetSpawnPoint( bool checkForOthers = true )
    {
        // Shuffle the spawnpoints so we can just pick them linearly later
        var spawnPoints = Game.ActiveScene.GetComponentInChildren<NetworkHelper>().SpawnPoints.Shuffle();
        
        var spawnPoint = spawnPoints.First().GetComponent<Shooter.SpawnPoint>();

        if ( checkForOthers )
        {
            var characters = Game.ActiveScene.GetComponentsInChildren<CharacterSpawner>();

            int ind = 1;
            while ( ind < spawnPoints.Count() )
            {
                if ( !CheckForNearCharacters( spawnPoint, characters ) ) break;

                // Will return the final one if no previous was valid
                // Should probably check for the best one in those as well or wait etc.
                spawnPoint = spawnPoints.ElementAt( ind ).GetComponent<Shooter.SpawnPoint>();
                ind++;

            }
            
        }

        return spawnPoint;

    }

    private static float MinCharacterDistance = 500.0f;

    private static bool CheckForNearCharacters( SpawnPoint spawnPoint, IEnumerable<Component> characters )
    {

        foreach ( var character in characters )
        {
            if ( spawnPoint.WorldPosition.DistanceSquared( character.WorldPosition ) < (MinCharacterDistance * MinCharacterDistance) )
            {
                return true;
            }
        }

        return false;

    }

}
