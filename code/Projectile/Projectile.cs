using Sandbox.Utility;

namespace Shooter;

/// <summary>
/// A simple class for e.g. bullets or rocket ammo.
/// </summary>
public sealed class Projectile : Component
{
	[Property] private float velocity = 1500.0f;
	[Property] public GameObject Attacker { get; set; }

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

        if ( objectHit.Root == Attacker )
		{
			return;
		}

		var blastEffect = new BlastEffect
		{
			BlastForce = 700.0f,
		};
		blastEffect.NetworkSpawn();
		blastEffect.TriggerBlast( WorldPosition, Attacker );

		// Just destroy for now
		DestroyGameObject();
	}
	

}
