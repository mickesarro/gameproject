using Sandbox;
using Shooter.Sounds;

namespace Shooter;

public enum BulletType { Bullet, Projectile };
public enum FireType { FullAuto, SemiAuto, Burst };
public enum AmmoType { Light, Medium, Heavy, Rocket }

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
	[Property, Group( "Fire" )] public SoundManager.SoundType FiringSound { get; private set; } 
		= SoundManager.SoundType.None;

	[Property, Group( "Magazine" )] public AmmoType AmmoType { get; private set; }
	[Property, Group( "Magazine" )] public int MaxAmmo { get; private set; } = 10;
	[Property, Group( "Magazine" )] public int AmmoLeft { get; set; } = 10;
	[Property, Group( "Magazine" )] public bool HasInfiniteAmmo { get; set; } = false;
	[Property, Group( "Magazine" )] public float LoadTime { get; private set; } = 1.0f;
	[Property, Group("Fire")] public float ChargeDuration { get; set; } = 0f; // 0 for normal weapons, !0 for Railgun

}
