using Sandbox;
using Shooter.Camera;

namespace Shooter;

/// <summary>
/// Defines details common with all game modes.
/// </summary>
public abstract class GameMode : Component, ISceneMetadata
{
    // Needs to be discussed how to implement these.
    // Should each one be a class inheriting this or e.g. a prefab of components.

    public static string Current { get; set; }

    public static void SetGameMode( GameMode gameMode )
    {
        if ( gameMode != null )
        {
            Current = gameMode.GameObject.PrefabInstanceSource;
        }
    }

    public static void SetGameMode( string gameMode ) => Current = gameMode;

    public virtual string ModeName { get; } = "GameMode";
    public abstract int ScoreLimit { get; }
    public abstract void WinCondition(PlayerStats latestScoreEvent);
    public abstract void DetermineWinners();

    public abstract int MaxPlayers { get; }
    public abstract int MinPlayers { get; }
    [Property] public virtual bool PopulateWithNPCs { get; set; } = true;

    public virtual string Objective { get; } = "Score points"; // Simple for now

    public virtual List<MapInstance> AvailableMaps { get; } = new();

    public virtual int StartCountdown { get; } = 5;

    public abstract CameraType Camera { get; }

    Dictionary<string, string> ISceneMetadata.GetMetadata()
    {
        return new() {
            { "Name", ModeName },
            { "ScoreLimit", ScoreLimit.ToString() },
            { "MaxPlayers", MaxPlayers.ToString() },
            { "MinPlayers", MinPlayers.ToString() },
            { "Objective", Objective }
        };
    }

}
