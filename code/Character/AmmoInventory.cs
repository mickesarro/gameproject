using System;
using Sandbox;

namespace Shooter;

/// <summary>
/// Manages characters ammunition inventory.
/// Add and remove return the amount that were added/removed.
/// </summary>
public sealed class AmmoInventory : Component
{
	[Property] private int MaxAmount { get; set; } = 999; // Currently just for all types

	// For detecting client side misuse or for future server authoring
	[Sync] private NetDictionary<AmmoType, int> Ammunition { get; set; } = new();

	private void SetAmmo( AmmoType type, int amount )
	{
		Ammunition[type] = amount;
	}

	/// <summary>
	/// Adds the amount of specified type of ammo to inventory.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="amount">Should be non-negative.</param>
	/// <returns>The amount actually added.</returns>
	public int AddAmmo( AmmoType type, int amount )
	{
		int current = AmmoCount( type );
		int canAdd = MaxAmount - current;

		if ( canAdd == 0 ) return 0;

		int added = Math.Min( amount, canAdd );

		SetAmmo( type, current + added );

		return added;
	}

	/// <summary>
	/// Removes the amount of specified type of ammo to inventory.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="amount">Should be non-negative.</param>
	/// <returns>The amount that was removed.</returns>
	public int RemoveAmmo( AmmoType type, int amount )
	{
		int current = AmmoCount( type );
		int removed = Math.Min( current, amount );

		SetAmmo( type, current - removed );

		return removed;
	}

	public int AmmoCount( AmmoType type )
	{
		return Ammunition.TryGetValue( type, out var count )
			? count
			: 0;
	}

	public bool HasAmmo( AmmoType type )
	{
		return AmmoCount( type ) > 0;
	}

    public int GetAmmo( AmmoType type  )
    {
        return Ammunition.GetValueOrDefault( type, 0 );
    }
}
