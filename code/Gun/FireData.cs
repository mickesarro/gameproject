using Sandbox;

public enum BulletType { Bullet, Projectile };
public enum FireType { FullAuto, SemiAuto, Burst };

/// <summary>
/// Holds the data specific to the firing functionality.
/// </summary>
public sealed class FireData : Component
{
	// Most of these don't actually yet do anything
	[Property, Group( "Fire" )] public FireType FireType { get; private set; } = FireType.FullAuto;

	[Property, Group( "Bullet" )] public BulletType BulletType { get; private set; } = BulletType.Bullet;
	[Property, Group( "Bullet" )] public BulletData BulletData { get; private set; }
	[Hide] public float Damage => BulletData.Damage;
	[Property, Group( "Fire" )] public int RPM { get; private set; } = 100;
	[Property, Group( "Fire" )] public float Spread { get; private set; } = 10f;
	[Property, Group( "Fire" )] public float Recoil { get; private set; } = 5f;

	[Property, Group( "Magazine" )] public float MaxAmmo { get; private set; } = 10f;
	[Property, Group( "Magazine" )] public float AmmoLeft { get; set; } = 10f;
	[Property, Group( "Magazine" )] public float LoadTime { get; private set; } = 1.0f;

}
