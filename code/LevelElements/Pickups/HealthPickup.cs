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

                // Makes sure that bots hitting the JumpPad don't play the sound to you locally
                if ( other.GameObject.Root.Components.TryGet<PlayerController>( out _ ) )
                {
                    SoundManager.PlayLocal( SoundManager.SoundType.HealthPack );
                }

			}

            // DestroyGameObject();
            HideForTime.HideFor();
		}
	}
}
