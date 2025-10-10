using Sandbox;

/// <summary>
/// Acts as a proxy for different player interactions.
/// </summary>
public sealed class PlayerInteraction : Component, IPlayerEvent
{
	[Property, RequireComponent] private PlayerController Player { get; set; }
	[Property, RequireComponent] private PlayerInventory PlayerInventory { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerInventory = GetComponent<PlayerInventory>();

		IPlayerEvent.Post( e => e.OnSpawn( GameObject ) );
	}

	void IPlayerEvent.OnSwitchItem( ICollectable collectable )
	{
		// Change animations etc.
	}

	void IPlayerEvent.OnItemAdded( ICollectable item )
	{
		//AddToInventory( item );
	}

	private void AddToInventory( ICollectable collectable )
	{
		// Might not be needed
	}

}
