
using Sandbox;

namespace Shooter;

internal static class Spawner
{
    public static void SpawnCharacter( string prefabPath, SpawnPoint spawnPoint, PlayerStats prevStats = null )
    {
        // !! This needs to be revamped, we should not need to rely on such ad hoc solution

        if ( prefabPath == null )
        {
            Log.Warning( "[CharacterSpawner] No character prefab set" );
            return;
        }

        var character = GameObject.Clone( prefabPath, transform: spawnPoint.WorldTransform, startEnabled: true );

        if ( character.Components.TryGet<PlayerStats>( out var stats ) )
        {
            stats.Accumulate( prevStats );
        }

        character.NetworkSpawn();

    }
	
}
