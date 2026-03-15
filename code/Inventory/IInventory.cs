using Sandbox;

namespace Shooter;

/// <summary>
/// Common interface for all inventories.
/// </summary>
public interface IInventory
{
	ICollectable CurrentItem { get; }
	IReadOnlyList<ICollectable> Items { get; }

	bool AddItem( ICollectable item );
	void RemoveItem( ICollectable item );
    ICollectable GetItem( int index );
	void ChangeCurrentItem( ICollectable collectable );
	void ChangeCurrentItem( int index );
}
