using Sandbox.Utility;

namespace Shooter;

/// <summary>
/// A simple class for e.g. bullets or rocket ammo.
/// </summary>
public sealed class Projectile : Component
{
	[Property] private float velocity = 1500.0f;
	[Property] public GameObject Attacker { get; set; }
	[Property] private GameObject explosionPrefab { get; set; }

	/// <summary>
	/// Clones the projectile prefab, sets the attacker, and spawns it on the network.
	/// </summary>
	public static Projectile Spawn( GameObject prefab, Transform spawnTransform, GameObject attacker )
	{
		var go = prefab.Clone( new CloneConfig( spawnTransform, startEnabled: false ) );
		var projectile = go.Components.Get<Projectile>( includeDisabled: true );
		projectile.Attacker = attacker;
		go.Enabled = true;
		go.NetworkSpawn();
		return projectile;
	}

	private Collider collider; // Can be used with different types of colliders

	protected override void OnAwake()
	{
		base.OnAwake();
		if ( Components.TryGet<Collider>( out var collider ) )
		{
			collider.OnObjectTriggerEnter = OnCollision;
		}
		else
		{
			Log.Warning( "No collider found" );
		}
	}

	protected override void OnFixedUpdate()
	{
		// Time.Delta should decouple this calculation from game fps T: chatgpt
		WorldPosition += WorldTransform.Forward * velocity * Time.Delta;
	}

	private void OnCollision( GameObject objectHit )
	{

        if ( IsProxy ) return;

        if ( objectHit.Root == Attacker )
		{
			return;
		}

		if ( explosionPrefab is null )
		{
			Log.Warning( "Projectile: explosionPrefab not set" );
			DestroyGameObject();
			return;
		}

		BlastEffect.Spawn( explosionPrefab, WorldPosition, Attacker );
		DestroyGameObject();
	}
	

}
