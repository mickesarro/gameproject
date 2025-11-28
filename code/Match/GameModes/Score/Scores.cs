using Sandbox;

namespace Shooter;

/// <summary>
/// Used to define score values for different events.
/// A gamemode can still count e.g. kills only without using the score amounts.
/// </summary>
public class Scores : Component
{
    // A simple one for now as DM does not need much else
    // Can be extended through inheritance
    [Property] public int Kill { get; private set; } = 100;
    [Property] public float CriticalMultiplier { get; private set; } = 1.5f;

    [Property] public int Assist { get; private set; } = 50;

    [Property] public int Objective { get; private set; } = 200;
}
