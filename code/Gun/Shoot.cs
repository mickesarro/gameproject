
/// <summary>
/// Component to enable shooting for guns
/// </summary>
public sealed class Shoot : Component
{

	[Property] private float loadTime = 1.0f;

	[Property] private GameObject projectilePrefab { get; set; }
	[Property] private GameObject barrelEnd; // Spawn point

	protected override void OnAwake()
	{
		if (projectilePrefab == null || barrelEnd == null)
		{
			Log.Warning( "Properties not set!" );
			DestroyGameObject();
		}
	}

	private float elapsed = 0.0f;
	protected override void OnUpdate()
	{
		// Should be implemented with some form of events perhaps
		if (Input.Down("attack1") && elapsed > loadTime)
		{
			projectilePrefab.Clone( barrelEnd.WorldTransform );
			elapsed = 0.0f;
		}
		elapsed += Time.Delta;
	}
}
