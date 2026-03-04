using Sandbox;
using static Sandbox.Component;
using Sandbox.Audio;
using Shooter.Sounds;

namespace Shooter;

/// <summary>
/// Simulates a blast created by projectile collission / other explosion
/// </summary>
public class BlastEffect : Component
{
	[Property] public float Radius { get; set; } = 300.0f;
	[Property] public float BlastForce { get; set; } = 500.0f;
	[Property] public float Damage { get; set; } = 100.0f;
	[Property, Range( 0f, 1f )] public float SelfDamageMultiplier { get; set; } = 0.1f;

	public GameObject Attacker { get; set; }

	/// <summary>
	/// Clones the explosion prefab, sets the attacker, spawns it on the network, and triggers the blast.
	/// </summary>
	public static BlastEffect Spawn( GameObject prefab, Vector3 position, GameObject attacker )
	{
		var go = prefab.Clone( new CloneConfig( new Transform( position ), startEnabled: false ) );
		var blast = go.Components.Get<BlastEffect>( includeDisabled: true );
		blast.Attacker = attacker;
		go.Enabled = true;
		go.NetworkSpawn();
		blast.TriggerBlast( position );
		return blast;
	}

	public void TriggerBlast( Vector3 position )
	{
		SoundManager.PlayGlobal( SoundManager.SoundType.Explosion, position, 6700f );

		if ( IsProxy )
		{
			DestroyGameObject();
			return;
		}

		bool IsPlayer = Attacker.GetComponent<ICharacterBase>().IsPlayer; // !! Quick solution for now

		foreach ( var hittable in Game.ActiveScene.FindInPhysics( new Sphere( position, Radius ) ) )
		{
			// Needed now as the hierarchy changed
			var hittableChar = hittable.Root;
			// If PlayerController, apply force
			// Something more generic such as hittable or the IDamageable interface would be better
			if ( hittableChar.Components.TryGet<ICharacterBase>( out var character ) )
			{
				var charWorldCenter = hittableChar.WorldPosition + Vector3.Up * 36f;

				var trace = Game.ActiveScene.Trace
					.Ray( position, charWorldCenter )
					.UseHitboxes( true )
					.WithoutTags( "movement" )
					.Run();

				if ( trace.Hit && trace.GameObject == hittableChar )
				{
					var distance = charWorldCenter.Distance( position );
					var damageFalloff = 1.0f - (distance / Radius).Clamp( 0.0f, 1.0f );
					var direction = (charWorldCenter - position).Normal;

					character.ApplyForce( direction * BlastForce * damageFalloff );

					float finalDamage = Damage * damageFalloff;

					if ( hittableChar == Attacker )
					{
						finalDamage *= SelfDamageMultiplier;
					}

					var damageInfo = new DamageInfo()
					{
						Damage = finalDamage,
						Attacker = Attacker,
						Position = trace.HitPosition,
						Origin = position,
					};
					damageInfo.Tags.Add( "explosion" );

					if ( !IsPlayer )
					{
						damageInfo.Tags.Add( "npc" );
					}

					IDamageEvent.Post( e => e.OnDamage( hittableChar, damageInfo ) );

					hittableChar.GetComponent<IDamageable>()
						?.OnDamage( damageInfo );
				}
			}
		}
		// DestroyGameObject();
	}
}
