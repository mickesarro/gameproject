using Sandbox;

/// <summary>
/// Acts as a proxy for different player interactions.
/// </summary>
public sealed class PlayerInteraction : Component, IPlayerEvent
{
	[Property, RequireComponent] private PlayerController Player { get; set; }
	[Property, RequireComponent] private Inventory PlayerInventory { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerInventory = GetComponent<Inventory>();
	}

	void IPlayerEvent.OnSwitchItem( ICollectable collectable )
	{
		// Change animations etc.
	}

	void IPlayerEvent.OnItemAdded( ICollectable item )
	{
		AddToInventory( item );
	}

	private void AddToInventory( ICollectable collectable )
	{
		PlayerInventory?.AddItem( collectable );
	}

}
