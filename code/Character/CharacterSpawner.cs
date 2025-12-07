using Sandbox;

namespace Shooter;

/// <summary>
/// Handles spawning and respawnign characters 
/// </summary>
public sealed class CharacterSpawner : Component
{
    // !! This needs to be revamped, we should not need to rely on such ad hoc solution
    // One way is to have one client side (not the static) spawner that can just spawn
    // the whole player prefab again

    [Property] private PrefabFile CharacterPrefab { get; set; }

    public void Spawn( SpawnPoint spawnPoint )
    {
        PlayerStats prevStats = GameObject.GetComponent<PlayerStats>(includeDisabled: true);
        Spawner.SpawnCharacter( CharacterPrefab, spawnPoint, prevStats );
        MatchStatsManager.Instance.RemovePreviousStats( prevStats );
        DestroyGameObject();
    }

	public void Respawn( SpawnPoint spawnPoint )
    {
        Spawn( spawnPoint );
    }
}
