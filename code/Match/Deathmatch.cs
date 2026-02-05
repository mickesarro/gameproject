using Sandbox;

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

    private MatchStatsManager statsManager;

    protected override void OnStart()
    {
        base.OnStart();

        statsManager = MatchStatsManager.Instance;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        //Log.Info( statsManager.Tracked.Count() );
    }

    //[Rpc.Host]
    // scoren lisäyksessä kestää aina hetki, parempi vertailla ensin
    public override void WinCondition( PlayerStats latestScoreEvent )
    {
        const int scoreToAdd = 1; // Define score amounts somewhere
        if ( latestScoreEvent.Score + scoreToAdd >= scoreLimit )
        {
            MatchManager.Instance.EndGame();
            GameObject.Enabled = false; // Might not work, but for dev time
            Log.Info( "Game ended by score!" );
        }
    }

    // protected override void OnUpdate()
    // {
    //     base.OnUpdate();
    //
    //     if (statsManager?.Top.Score >= scoreLimit)
    //     {
    //         // Match end handler / ending screen etc. needs to be made
    //         IMatchEvents.Post( e => e.OnGameEnd() );
    //
    //         GameObject.Enabled = false; // Might not work, but for dev time
    //
    //         Log.Info( "Game ended by score!" );
    //     }
    // }
}
