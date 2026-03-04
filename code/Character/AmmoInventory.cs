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
    //[Sync] private NetDictionary<AmmoType, int> Ammunition { get; set; } = new();

    public class AmmoData { public int Current = 0, Maximum = 0; };
    private Dictionary<AmmoType, AmmoData> Ammunition { get; set; } = new();

    private void SetAmmo( AmmoType type, int amount, int maxAmmo )
	{
        if ( !Ammunition.TryGetValue( type, out var _ ) )
        {
            Ammunition.Add( type, new() { Current = 0, Maximum = maxAmmo } );
        }

		Ammunition[type].Current = amount;
	}

	/// <summary>
	/// Adds the amount of specified type of ammo to inventory.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="amount">Should be non-negative.</param>
	/// <returns>The amount actually added.</returns>
	public int AddAmmo( AmmoType type, int amount )
	{
		return AddAmmo( type, amount, MaxAmount );
	}

    public int AddAmmo( AmmoType type, int amount, int maxAmmo )
    {
        int current = AmmoCount( type );
        int canAdd = maxAmmo - current;
        if ( Ammunition.TryGetValue( type, out var ammoData ) )
        {
            canAdd = ammoData.Maximum - ammoData.Current;
        }
        

        if ( canAdd == 0 ) return 0;

        int added = Math.Min( amount, canAdd );

        SetAmmo( type, current + added, maxAmmo );

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

		SetAmmo( type, current - removed, MaxAmount );

		return removed;
	}

	public int AmmoCount( AmmoType type )
	{
		return Ammunition.TryGetValue( type, out var count )
			? count.Current
            : 0;
	}

	public bool HasAmmo( AmmoType type )
	{
		return AmmoCount( type ) > 0;
	}
}
