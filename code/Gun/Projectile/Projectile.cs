
using Sandbox.Utility;

/// <summary>
/// A simple class for e.g. bullets or rocket ammo.
/// </summary>
public sealed class Projectile : Component
{
	[Property] private float velocity = 1100f;

	private CapsuleCollider collider;

	protected override void OnAwake()
	{
		base.OnAwake();
		collider = GetComponent<CapsuleCollider>();
		if ( collider != null)
		{
			collider.OnObjectTriggerEnter = OnObjectTriggerEnter;
		} else
		{
			Log.Warning( "No collider found" );
		}
	}

	protected override void OnFixedUpdate()
	{
		// Time.Delta should decouple this calculation from game fps T: chatgpt
		WorldPosition += WorldTransform.Forward * velocity * Time.Delta;
	}

	private void OnObjectTriggerEnter( GameObject objectHit )
	{
		Log.Info("Object hit tags: " + objectHit.Tags );
		OnCollision( objectHit );
	}

	private void OnCollision(GameObject objectHit)
	{
		Log.Info("Projectile tags: " + collider.Tags);
		if ( objectHit.Tags.Contains( Steam.SteamId.ToString() ))
		{
			return;
		}
		// Just destroy for now
		DestroyGameObject();
	}
	

}
