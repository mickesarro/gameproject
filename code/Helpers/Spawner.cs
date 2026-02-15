
using Sandbox;
using System;

namespace Shooter;

internal static class Spawner
{
    public static void SpawnCharacter(
        PrefabFile prefab, SpawnPoint spawnPoint = null, PlayerStats prevStats = null, string name = null
    ) {

        if ( !prefab.IsValid() )
        {
            Log.Warning( "[CharacterSpawner] No character prefab set" );
            return;
        }

        // Sometimes fails to fetch earlier
        spawnPoint ??= GetSpawnPoint();

        var character = GameObject.Clone( prefab, name: name, transform: spawnPoint.WorldTransform, startEnabled: true );
        
        if ( prevStats != null && character.Components.TryGet<PlayerStats>( out var stats ) )
        {
            stats.Accumulate( prevStats );
        }

        character.NetworkSpawn();
    }

    /// <summary>
    /// Spawn a character from a gameobject
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="spawnPoint"></param>
    /// <param name="prevStats"></param>
    /// <param name="connection"
    /// <param name="name"></param>
    public static void SpawnCharacter(
        GameObject prefab,
        SpawnPoint spawnPoint = null,
        PlayerStats prevStats = null,
        Connection connection = null,
        string name = null
    ) {

        if ( !prefab.IsValid() )
        {
            Log.Warning( "[CharacterSpawner] No character prefab set" );
            return;
        }

        spawnPoint ??= GetSpawnPoint();

        var character = prefab.Clone( name: name, transform: spawnPoint.WorldTransform, startEnabled: true );

        if ( prevStats != null && character.Components.TryGet<PlayerStats>( out var stats ) )
        {
            stats.Accumulate( prevStats );
        }

        character.NetworkSpawn( connection ?? Connection.Local );
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

        var spawnPoint = spawnPoints.First();

        if ( checkForOthers )
        {
            var characters = Game.ActiveScene.GetComponentsInChildren<CharacterSpawner>();

            int ind = 1;
            while ( ind < spawnPoints.Count() )
            {
                if ( !CheckForNearCharacters( spawnPoint, characters ) ) break;

                // Will return the final one if no previous was valid
                // Should probably check for the best one in those as well or wait etc.
                spawnPoint = spawnPoints.ElementAt( ind );
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
