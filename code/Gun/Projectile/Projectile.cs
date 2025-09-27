
/// <summary>
/// A simple class for e.g. bullets or rocket ammo.
/// </summary>
public sealed class Projectile : Component
{

	[Property] private float velocity = 20f;

	private Collider collider;

	protected override void OnAwake()
	{
		base.OnAwake();
		collider = GetComponent<Collider>();
		if ( collider != null)
		{
			collider.OnTriggerEnter += OnCollision;
		} else
		{
			Log.Warning( "No collider found" );
		}
	}

	protected override void OnFixedUpdate()
	{
		WorldPosition += WorldTransform.Forward * velocity;
	}


	private void OnCollision(Collider collider)
	{
		// So it doesn't collide immediately
		// !! FOR TESTING ONLY
		if ( collider.Tags.Contains( "player" ) )
		{
			return;
		}
		// Just destroy for now
		DestroyGameObject();
	}
	

}
