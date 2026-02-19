using Sandbox;
using static Sandbox.Component;
using Sandbox.Audio;
using Shooter.Sounds;

namespace Shooter;

/// <summary>
/// Simulates a blast created by projectile collission / other explosion
/// </summary>
public class BlastEffect : GameObject
{
	[Property] public float Radius { get; set; } = 300.0f; // Tweak the blast radius
	[Property] public float BlastForce { get; set; } = 500.0f; // Tweak blast force
	[Property] public float Damage { get; set; } = 100.0f; // Tweak damage
	[Property, Range(0f, 1f)] public float SelfDamageMultiplier {get; set;} = 0.1f;
	
	public void TriggerBlast( Vector3 position, GameObject attacker )
	{
		SoundManager.PlayGlobal( SoundManager.SoundType.Explosion, position, 5000f );

		var sphere = new Sphere( position, Radius );

        bool IsPlayer = attacker.GetComponent<ICharacterBase>().IsPlayer; // !! Quick solution for now
		
		foreach ( var hittable in Game.ActiveScene.FindInPhysics( sphere ) )
		{
			// If PlayerController, apply force
			// Something more generic such as hittable or the IDamageable interface would be better
			if ( hittable.Components.TryGet<ICharacterBase>( out var character ) )
			{
				// var hittable = hittable.GetComponent<PlayerController>() as PlayerController;

				var charWorldCenter = hittable.WorldPosition.WithZ( hittable.WorldPosition.z + 36 );
				var direction = (charWorldCenter - position).Normal; // Normalized direction vector
				var distance = charWorldCenter.Distance(position);
				var damageFalloff = 1.0f - (distance / Radius).Clamp(0.0f, 1.0f);
				
				var trace = Game.ActiveScene.Trace
					.Ray( position, charWorldCenter )
					.Run();

				if (trace.Hit && trace.GameObject == hittable)
				{
					character.ApplyForce(direction * BlastForce * damageFalloff);

					float finalDamage = Damage * damageFalloff;

                    if ( hittable == attacker )
                    {
                        finalDamage *= SelfDamageMultiplier;
                    }

					var damageInfo = new DamageInfo()
					{
						Damage = finalDamage,
						Attacker = attacker,
						Position = trace.HitPosition,
					};

                    if ( !IsPlayer )
                    {
                        damageInfo.Tags.Add( "npc" );
                    }

                    IDamageEvent.Post( e => e.OnDamage( hittable, damageInfo ) );

					hittable.GetComponent<IDamageable>()
						?.OnDamage( damageInfo );
				}
			}
		}
		Destroy();
	}
}
