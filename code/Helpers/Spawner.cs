
using Sandbox;

namespace Shooter;

internal static class Spawner
{
    public static void SpawnCharacter( PrefabFile prefab, SpawnPoint spawnPoint, PlayerStats prevStats = null )
    {
        // !! This needs to be revamped, we should not need to rely on such ad hoc solution

        if ( prefab == null )
        {
            Log.Warning( "[CharacterSpawner] No character prefab set" );
            return;
        }

        var character = GameObject.Clone( prefab, transform: spawnPoint.WorldTransform, startEnabled: true );

        if ( character.Components.TryGet<PlayerStats>( out var stats ) )
        {
            stats.Accumulate( prevStats );
        }

        character.NetworkSpawn();

    }
	
}
