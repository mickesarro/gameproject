using Sandbox;
using System;

namespace Shooter;

public enum InventorySlot
{
    First = 0,
    Second = 1,
    Third = 2,
	Fourth = 3,
    Count, // Dynamically assigns the value
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
	private readonly ICollectable[] weapons = new ICollectable[(int)WeaponType.Total]; // Plus 1 for empty
	public IReadOnlyList<ICollectable> Items => weapons;
    
    public ICollectable GetItem( int slot )
        => slot < weapons.Length ? weapons[slot] : default;

    [Property] public AmmoInventory AmmoInventory { get; private set; }

    protected override void OnStart()
    {
        base.OnStart();

        AmmoInventory ??= GetComponent<AmmoInventory>();
    }

	/// <summary>
	/// Adds an item to the players inventory.
	/// Needs to be also implement IWeapon so that it can have a WeaponType.
	/// </summary>
	/// <param name="item"></param>
	public bool AddItem( ICollectable item )
    {
        if ( IsProxy ) return false;
        
		if ( item == null ) return false;

		if ( item is not IWeapon weapon || weapon.WeaponType == WeaponType.Total ) return false;

		int ind = (int)weapon.WeaponType;

		var current = weapons[ind];
		if ( current != null && current != item )
		{
            if ( weapon.WeaponType != WeaponType.Melee )
            {
                var firedata = weapon.GunData.PrimaryFireData;
                //AmmoInventory.AddAmmo( firedata.AmmoType, firedata.PickupAmmo, firedata.MaxAmmo );
                // This is now an ugly solution for avoiding ammo inventory
                // If this is to remain permanent, a better one should be made
                var currFiredata = ((IWeapon)current).GunData.PrimaryFireData;
                var ammoToAdd = Math.Min( currFiredata.MaxAmmo - currFiredata.AmmoLeft, firedata.PickupAmmo );

                currFiredata.AmmoLeft += ammoToAdd;
                firedata.AmmoLeft -= ammoToAdd;

                IPlayerEvent.Post( e => e.OnAmmoAdded( (IWeapon)current, ammoToAdd ) );
            }
            DropWeapon( item );
            return false;
			//DropWeapon( current );
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

	/// <summary>
    /// Clears all weapons from the player's inventory EXCEPT the melee weapon
    /// </summary>
	public void Clear()
    {
        if ( IsProxy ) return;

        for ( int i = 0; i < weapons.Length - 1; i++ )
        {
            if ( weapons[i] != null )
            {
                RemoveItem( i );
            }
        }

		// Automatically equip melee after clearing
		if (CurrentItem == null )
		{
			ChangeCurrentItem( InventorySlot.Next );
		}
    }

	void IPlayerEvent.OnSwitchItem( ICollectable collectable ) => ChangeCurrentItem( collectable );
	void IPlayerEvent.OnSwitchItem( InventorySlot slot ) => ChangeCurrentItem( slot );

	/// <summary>
	/// Changes the current item in the players hand.
	/// </summary>
	/// <param name="ind"></param>
	public void ChangeCurrentItem( int ind )
	{
        if ( IsProxy ) return;
        
        if ( ind < 0 || ind >= weapons.Length ) return;

        if ( weapons[ind] == null ) return;

        // Disable previous weapon safely
        CurrentItem?.EnableGo( false );

        // Set new weapon
        //Log.Info( "set weapon" );
        CurrentItem = weapons[ind];
		currentSlot = ind;

        // Uses null propagation to safelu set it
        // Also all ICollectable implementing interfaces have EnableGo method
        CurrentItem?.EnableGo( true );
	}
    
	public void ChangeCurrentItem( ICollectable collectable ) {
        if ( IsProxy ) return;
		if ( collectable is not IWeapon weapon ) return;
		ChangeCurrentItem( (int)weapon.WeaponType );
	}

	public void ChangeCurrentItem( InventorySlot slot )
	{
        if ( IsProxy ) return;

        int ind;
        switch ( slot )
		{
			case InventorySlot.Next:
				ind = currentSlot + 1 < weapons.Length ? currentSlot + 1 : 0;
                if ( weapons[ind] == null ) ind = FindNextWeapon( ind, slot );
                ChangeCurrentItem( ind );
                break;
			case InventorySlot.Previous:
                ind = currentSlot - 1 >= 0 ? currentSlot - 1 : weapons.Length - 1;
                if ( weapons[ind] == null ) ind = FindNextWeapon( ind, slot );
                ChangeCurrentItem( ind );
				break;
			default:
				ChangeCurrentItem( (int)slot );
				break;
		}
	}

    private int FindNextWeapon( int ind, InventorySlot inventorySlot )
    {
        int dir = inventorySlot == InventorySlot.Next ? 1 : -1;

        while ( ind > 0 && (ind < weapons.Length - 1) && weapons[ind] == null )
        {
            ind += dir;
        }

        if ( weapons[ind] == null ) ind = 3;

        return ind;
    }

	private void DropWeapon( ICollectable weapon )
	{
        // This is work in progress, need to add a drop/transfer method
        // to the collectables to handle this well.

        // Handle dropping the weapon if deemed necessary
        var go = weapon.GetGameObject();

        // For now just destroy so we don't pick weapons of same type
        go.Destroy();
        return;

        // go.GetComponent<IWeapon>().User = null;
        go.Parent = null;
        go.Network.DropOwnership();

	}

}

