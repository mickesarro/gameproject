using Sandbox;

namespace Shooter;

/// <summary>
/// Bullet specific data.
/// </summary>
public sealed class BulletData : Component
{
	[Property] public float Damage { get; private set; } = 10.0f;

	[Property, Group( "Fire" )] public GameObject Tracer { get; private set; } = null;

	[Description("This is required only for projectiles, not hitscan bullets.")]
	[Property] public GameObject ProjectilePrefab { get; private set; }
}
