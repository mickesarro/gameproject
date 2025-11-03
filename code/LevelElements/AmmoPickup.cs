namespace Shooter;

public sealed class AmmoPickup : Component, Component.ITriggerListener
{
	[Property] private AmmoType AmmoType { get; set; }
	[Property] private int Amount { get; set; }

	public void OnTriggerEnter( Collider other )
	{
		if ( other.Components.TryGet<AmmoInventory>( out var ammoInv ) )
		{
			if ( !other.IsProxy )
			{
				ammoInv.AddAmmo(AmmoType, Amount);
			}

			DestroyGameObject();
		}
	}

}
