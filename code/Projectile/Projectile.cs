
using Sandbox.Utility;

/// <summary>
/// A simple class for e.g. bullets or rocket ammo.
/// </summary>
public sealed class Projectile : Component
{
	[Property] private float velocity = 1100f;

	private Collider collider; // Can be used with different types of colliders

	protected override void OnAwake()
	{
		base.OnAwake();
		collider = GetComponent<Collider>();
		if ( collider != null )
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

		Log.Info( "Projectile tags: " + collider.Tags );
		if ( objectHit.Tags.Contains( Steam.SteamId.ToString() ) )
		{
			return;
		}

		var blastEffect = new BlastEffect
		{
			BlastForce = 500.0f,
		};
		blastEffect.NetworkSpawn();
		blastEffect.TriggerBlast( WorldPosition );

		// Just destroy for now
		DestroyGameObject();
	}
	

}
