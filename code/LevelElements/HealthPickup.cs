using Sandbox;
using System;
using Shooter.Sounds;

namespace Shooter;

public sealed class HealthPickup : Component, Component.ITriggerListener
{
	[Property] private float HealthAmount { get; set; } = 50f;

    [Property] private HideForTime hideForTime;

    protected override void OnAwake()
    {
        base.OnAwake();

        hideForTime ??= GetOrAddComponent<HideForTime>();
    }

    public void OnTriggerEnter( Collider other )
	{
		if ( other.Components.TryGet<CharacterHealth>( out var healthComp ) && !hideForTime.IsHiding())
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
            hideForTime.HideFor();
		}
	}
}