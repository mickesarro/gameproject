using Sandbox;
using System;
using Shooter.Sounds;

namespace Shooter;

public sealed class HealthPickup : Pickup, Component.ITriggerListener
{
	[Property] private int HealthAmount { get; set; } = 50;
    [Property] private float Spin { get; set; } = 90f;

    [Rpc.Broadcast]
    private void HideBroadcast()
    {
        HideForTime.HideFor();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        GameObject.WorldRotation *= Rotation.FromYaw( Spin * Time.Delta );
    }

    public void OnTriggerEnter( Collider other )
	{
        if ( other.IsProxy ) return;

		if ( other.GameObject.Root.Components.TryGet<CharacterHealth>( out var healthComp ) && !HideForTime.IsHiding())
		{
            if (healthComp.Health >= healthComp.MaxHealth) {
                    return;
            }

            healthComp.AddHealth( HealthAmount );

            // Makes sure that bots hitting the JumpPad don't play the sound to you locally
            if ( other.Tags.Contains( "player" ) )
            {
                SoundManager.PlayLocal( SoundManager.SoundType.HealthPack );
            }

            // DestroyGameObject();
            HideBroadcast();

        }
	}
}
