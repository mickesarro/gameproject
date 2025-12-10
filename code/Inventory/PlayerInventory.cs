using Sandbox;

namespace Shooter;

public enum InventorySlot
{
    First = 0,
    Second = 1,
    Third = 2,
	Next,
	Previous,
}

/// <summary>
/// Handles players slot based inventory.
/// Has primary, secondary and melee slots.
/// </summary>
public sealed class PlayerInventory : Component, IInventory, IPlayerEvent
{
	// Weapons slots
	public ICollectable PrimaryWeapon => weapons[0];
	public ICollectable SecondaryWeapon => weapons[1];
	public ICollectable MeleeWeapon => weapons[2];
    public ICollectable Empty => weapons[3];

    public ICollectable CurrentItem { get; private set; } = null;
	public IWeapon CurrentWeapon => (IWeapon)CurrentItem;
	private int currentSlot = -1;

	// The actual collection of the weapons
	// Dictionary would add overhead for such small array, but also would be more type safe.
	private readonly ICollectable[] weapons = new ICollectable[(int)WeaponType.Total + 1]; // Plus 1 for empty
	public IEnumerable<ICollectable> Items => weapons;

	/// <summary>
	/// Adds an item to the players inventory.
	/// Needs to be also implement IWeapon so that it can have a WeaponType.
	/// </summary>
	/// <param name="item"></param>
	public bool AddItem( ICollectable item )
	{
		if ( item == null ) return false;

		if ( item is not IWeapon weapon || weapon.WeaponType == WeaponType.Total ) return false;

		int ind = (int)weapon.WeaponType;

		var current = weapons[ind];
		if ( current != null && current != item )
		{
			DropWeapon( current );
		}

		weapons[ind] = item;

		if (CurrentItem == null || currentSlot == ind )
		{
			ChangeCurrentItem( item );
		}

        return true;
	}

	void IPlayerEvent.OnItemAdded( ICollectable item ) => AddItem( item );

	void IPlayerEvent.OnWeaponAdded( IWeapon item ) => AddItem( item as ICollectable );

	/// <summary>
	/// Removes the specified item from the inventory.
	/// </summary>
	/// <param name="item"></param>
	public void RemoveItem( ICollectable item )
	{
		for (int i = 0; i < weapons.Length; ++i )
		{
			if (weapons[i] == item )
			{
				RemoveItem( i );
			}
		}
	}

	public void RemoveItem( int ind )
	{
		if ( ind < 0 || ind >= weapons.Length ) return;

		if ( weapons[ind] == CurrentItem )
		{
			DropWeapon( CurrentItem );
			CurrentItem = null;
		}

		weapons[ind] = null;
	}

	void IPlayerEvent.OnSwitchItem( ICollectable collectable ) => ChangeCurrentItem( collectable );
	void IPlayerEvent.OnSwitchItem( InventorySlot slot ) => ChangeCurrentItem( slot );

	/// <summary>
	/// Changes the current item in the players hand.
	/// </summary>
	/// <param name="ind"></param>
	public void ChangeCurrentItem( int ind )
	{
        if ( ind < 0 || ind >= weapons.Length ) return;

        // Disable previous weapon safely
        CurrentItem?.EnableGo( false );

        if ( weapons[ind] == null ) ind = 3;

		// Set new weapon
		CurrentItem = weapons[ind];
		currentSlot = ind;

        // Uses null propagation to safelu set it
        // Also all ICollectable implementing interfaces have EnableGo method
        CurrentItem?.EnableGo( true );
	}


	public void ChangeCurrentItem( ICollectable collectable ) {
		if ( collectable is not IWeapon weapon ) return;
		ChangeCurrentItem( (int)weapon.WeaponType );
	}

	public void ChangeCurrentItem( InventorySlot slot )
	{
		switch ( slot )
		{
			case InventorySlot.Next:
				ChangeCurrentItem( currentSlot + 1 < weapons.Length ? currentSlot + 1 : 0 );
				break;
			case InventorySlot.Previous:
				ChangeCurrentItem( currentSlot - 1 >= 0 ? currentSlot - 1 : weapons.Length - 1 );
				break;
			default:
				ChangeCurrentItem( (int)slot );
				break;
		}
	}

	private void DropWeapon( ICollectable weapon )
	{
        // This is work in progress, need to add a drop/transfer method
        // to the collectables to handle this well.

        // Handle dropping the weapon if deemed necessary
        var go = weapon.GetGameObject();
        // go.GetComponent<IWeapon>().User = null;
        go.Parent = null;
        go.Network.DropOwnership();

	}

}
