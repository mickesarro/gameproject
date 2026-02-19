using Shooter.Sounds;

namespace Shooter;

public sealed class AmmoPickup : Component, Component.ITriggerListener
{
	[Property] private AmmoType AmmoType { get; set; }
	[Property] private int Amount { get; set; }

    [Property] private HideForTime hideForTime;

    protected override void OnAwake()
    {
        base.OnAwake();

        hideForTime ??= GetOrAddComponent<HideForTime>();
    }

    public void OnTriggerEnter( Collider other )
	{
		if ( other.GameObject.Root.Components.TryGet<AmmoInventory>( out var ammoInv ) && !hideForTime.IsHiding() )
		{
			if ( !other.IsProxy )
			{
				ammoInv.AddAmmo(AmmoType, Amount);

				//Play reload sound
				SoundManager.PlayLocal(SoundManager.SoundType.Reload);
			}

            // DestroyGameObject();
            hideForTime.HideFor();
		}
	}

}
