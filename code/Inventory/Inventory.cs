using System.Collections.Generic;
using Sandbox;

/// <summary>
/// Represents a generic inventory for items.
/// </summary>
public class Inventory : Component
{
	[Property] private GameObject Owner { get; set; }

	private readonly List<ICollectable> inventoryItems = new();

	public ICollectable CurrentItem { get; private set; } = null;

	protected override void OnAwake()
	{
		base.OnAwake();
		Owner ??= GameObject;
	}

	public List<ICollectable> GetItems()
	{
		return inventoryItems;
	}

	/// <summary>
	/// Adds a new item to inventory.
	/// If this is the first item, sets it as CurrentItem.
	/// </summary>
	/// <param name="item"></param>
	public void AddItem( ICollectable item )
	{
		inventoryItems.Add( item );

		if (CurrentItem == null)
		{
			ChangeCurrentItem( item );
		}
	}

	/// <summary>
	/// Removes the item from inventory.
	/// Sets new CurrentItem if removed was it.
	/// </summary>
	/// <param name="item"></param>
	public void RemoveItem( ICollectable item )
	{
		inventoryItems.Remove( item );
		if ( CurrentItem == item )
		{
			ChangeCurrentItem( inventoryItems.LastOrDefault() );
		}
	}

	public void ChangeCurrentItem( ICollectable item )
	{
		if ( item != null )
		{
			CurrentItem?.EnableGo( false );
			CurrentItem = item;
			CurrentItem.EnableGo( true );

			IPlayerEvent.PostToGameObject( GameObject, e => e.OnSwitchItem( item ) );
			
		}
	}

	public void ChangeCurrentItem( int index )
	{
		if ( index >= 0 && index < inventoryItems.Count )
		{
			ChangeCurrentItem( inventoryItems[index] );
		}
	}
}
