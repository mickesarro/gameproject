using Sandbox;

/// <summary>
/// Bullet specific data.
/// </summary>
public sealed class BulletData : Component
{
	[Property] public float Damage { get; private set; } = 10.0f;

	[Description("This is required only for projectiles, not hitscan bullets.")]
	[Property] public GameObject ProjectilePrefab { get; private set; }
}
