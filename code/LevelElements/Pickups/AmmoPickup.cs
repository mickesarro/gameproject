using Shooter.Sounds;

namespace Shooter;

public sealed class AmmoPickup : Pickup, Component.ITriggerListener
{
	[Property] private AmmoType AmmoType { get; set; }
	[Property] private int Amount { get; set; }

    public void OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Root.Components.TryGet<AmmoInventory>( out var ammoInv ) && !HideForTime.IsHiding() )
		{
			if ( !other.IsProxy )
			{
				ammoInv.AddAmmo(AmmoType, Amount);

				//Play reload sound
				SoundManager.PlayLocal(SoundManager.SoundType.Reload);
			}

            // DestroyGameObject();
            HideForTime.HideFor();
		}
	}

}
