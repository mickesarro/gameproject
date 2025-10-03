using Sandbox;

public enum ShootType { Bullet, Projectile };

public sealed class GunData : Component
{

	[Property] public ShootType ShootType { get; private set; } = ShootType.Bullet;

	[Property] public BulletData BulletData { get; private set; }
	[Property] public GameObject BarrelEnd { get; private set; } // Spawn point
	[Property] public float LoadTime { get; private set; } = 1.0f;

	[Property] public GameObject Viewmodel { get; private set; }
}
