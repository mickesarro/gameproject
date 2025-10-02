using Sandbox;
using static Sandbox.Component;

/// <summary>
/// Simulates a blast created by projectile collission / other explosion
/// </summary>
public class BlastEffect : GameObject
{
	[Property] public float Radius { get; set; } = 200.0f; // Tweak the blast radius
	[Property] public float BlastForce { get; set; } = 500.0f; // Tweak blast force
	[Property] public float Damage { get; set; } = 50.0f; // Ehkä joskus joku damage mikä riippuu etäisyydestä?
	
	public void TriggerBlast( Vector3 position )
	{
		//PlayBlastEffect(position); // Placeholder for visual/audio effects

		var sphere = new Sphere( position, Radius );
		
		foreach ( var obj in Game.ActiveScene.FindInPhysics( sphere ) )
		{
			// If PlayerController, apply force
			if ( obj.GetComponent<PlayerController>() is PlayerController player )
			{
				var playerHitbox = player.GetComponent<ManualHitbox>();
				var hitboxCenter = playerHitbox.WorldPosition.WithZ( playerHitbox.WorldPosition.z + playerHitbox.CenterB.z / 2 );

				var direction = (hitboxCenter - position).Normal;  // Normalized direction vector
				var distance = player.WorldPosition.Distance(position);
				var damageFalloff = 1.0f - (distance / Radius).Clamp(0.0f, 1.0f);

				Ray hitRay = new Ray(position, direction);
				var sphereTrace = Game.ActiveScene.Trace
					.UseHitboxes(true)
					.Sphere(1.0f, hitRay, distance)
					.Run();

				if (sphereTrace.Hit && sphereTrace.GameObject == player.GameObject)
				{
					player.Punch(direction * BlastForce * damageFalloff);
					
					// !! For future reference
					player.GetComponent<IDamageable>()
						?.OnDamage(new DamageInfo()
						{
							Damage = Damage * damageFalloff,
							Attacker = this, // Just a placeholder, need to pass the actual attacker, if at all necessary
							Position = sphereTrace.HitPosition,
						}
					);

				}
			}
		}
		Destroy();
	}
}
