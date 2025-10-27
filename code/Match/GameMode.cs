using Sandbox;

/// <summary>
/// Defines details common with all game modes.
/// </summary>
public class GameMode : Component
{
	// Needs to be discussed how to implement these.
	// Should each one be a class inheriting this or e.g. a prefab of components.

	[Property] public string ModeName { get; private set; }

}
