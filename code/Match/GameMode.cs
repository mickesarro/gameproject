using Sandbox;

namespace Shooter;

/// <summary>
/// Defines details common with all game modes.
/// </summary>
public abstract class GameMode : Component
{
    // Needs to be discussed how to implement these.
    // Should each one be a class inheriting this or e.g. a prefab of components.

    public virtual string ModeName { get; } = "GameMode";
    public abstract int ScoreLimit { get; }

    public abstract int MaxPlayers { get; }
    public abstract int MinPlayers { get; }

    public virtual string Objective { get; } = "Score points"; // Simple for now


}
