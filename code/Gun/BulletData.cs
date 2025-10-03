using Sandbox;

public sealed class BulletData : Component
{
	[Property] public float Damage { get; private set; } = 10.0f;

	[Property] public GameObject ProjectilePrefab { get; private set; }
}
