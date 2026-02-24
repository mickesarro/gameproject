using Sandbox;
using System;
using Shooter.Sounds;

namespace Shooter;

public sealed class HealthPickup : Pickup, Component.ITriggerListener
{
	[Property] private float HealthAmount { get; set; } = 50f;


    public void OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Root.Components.TryGet<CharacterHealth>( out var healthComp ) && !HideForTime.IsHiding())
		{
            if (healthComp.Health >= healthComp.MaxHealth) {
                return;
            }
			if ( !other.IsProxy) 
			{
				healthComp.Health = Math.Min( healthComp.Health + HealthAmount, healthComp.MaxHealth );

				//Play reload sound
				SoundManager.PlayLocal(SoundManager.SoundType.Reload);
			}

            // DestroyGameObject();
            HideForTime.HideFor();
		}
	}
}
