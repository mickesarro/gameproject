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
	
	public void TriggerBlast( Vector3 position, GameObject attacker )
	{
		//PlayBlastEffect(position); // Placeholder for visual/audio effects

		var sphere = new Sphere( position, Radius );
		
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

					var damageInfo = new DamageInfo()
					{
						Damage = Damage * damageFalloff,
						Attacker = attacker,
						Position = trace.HitPosition,
					};

					IDamageEvent.Post( e => e.OnDamage( hittable, damageInfo ) );

					hittable.GetComponent<IDamageable>()
						?.OnDamage( damageInfo );
				}
			}
		}
		Destroy();
	}
}
