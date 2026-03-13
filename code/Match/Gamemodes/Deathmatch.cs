using Sandbox;
using Shooter.Camera;

namespace Shooter;

public sealed class Deathmatch : GameMode
{
    [Property] private string modeName { get; set; }
    public override string ModeName => modeName;

    [Property] private int scoreLimit { get; set; } = 10;
    public override int ScoreLimit => scoreLimit;

    [Property] private int maxPlayers { get; set; } = 12;
    public override int MaxPlayers => maxPlayers;

    [Property] private int minPlayers { get; set; } = 2;
    public override int MinPlayers => minPlayers;

    [Property] private CameraType CameraType { get; set; } = CameraType.Orbit;
    public override CameraType Camera => CameraType;

    [Rpc.Host]
    public override void WinCondition( PlayerStats latestScoreEvent )
    {
        if ( latestScoreEvent.IsValid() && latestScoreEvent.Score >= scoreLimit )
        {
            DetermineWinners();
            MatchManager.Instance.EndGame();
            GameObject.Enabled = false; // Might not work, but for dev time
            Log.Info( "Game ended by score!" );
        }
    }

    public override void DetermineWinners()
    {
        if ( !Networking.IsHost || MatchStatsManager.Instance == null ) return;

        var winners = MatchStatsManager.Instance.Tracked
            .OrderByDescending( x => x?.Score ).Take(3);

        foreach ( var winner in winners )
        {
            if ( winner?.GameObject?.IsValid() == true )
            {
                winner?.IncrementWins();
            }
        }

    }

    protected override void OnStart()
    {
        base.OnStart();
        
        AvailableMaps.Add(
            Scene.Get<MapInstance>()
        );
    }
}
