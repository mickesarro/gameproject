using System.Collections.Generic;
using Sandbox;

namespace Shooter;

/// <summary>
/// Represents a generic inventory for items.
/// </summary>
public class Inventory : Component, IInventory
{
	// !!NOTE: This as of now is not very useful for this game,
	// but either modifying this or inheriting, a generic box etc.
	// inventory can be made.

	[Property] private GameObject Owner { get; set; }

	private readonly List<ICollectable> inventoryItems = new();

	public ICollectable CurrentItem { get; private set; } = null;

	public IReadOnlyList<ICollectable> Items => inventoryItems;

    public ICollectable GetItem( int slot )
        => slot < inventoryItems.Count ? inventoryItems[slot] : default;

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
	public bool AddItem( ICollectable item )
	{
		inventoryItems.Add( item );

		if (CurrentItem == null)
		{
			ChangeCurrentItem( item );
		}
        return true;
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
