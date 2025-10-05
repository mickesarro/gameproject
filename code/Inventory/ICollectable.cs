
/// <summary>
/// Allows GameObject to become collectable.
/// </summary>
public interface ICollectable
{
	public string Name { get; set; }

	/// <summary>
	/// Handles the per-item collect logic.
	/// </summary>
	/// <param name="interactor">The GameObject that collects the item.</param>
	public void Collect( GameObject interactor );

	// This allows enabling and disabling selected items in inventory
	public void EnableGo( bool enabled );

}
