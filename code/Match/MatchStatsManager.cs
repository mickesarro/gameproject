using System;
using Sandbox;
using Sandbox.VR;

namespace Shooter;

/// <summary>
/// Central authority on logging match stats
/// </summary>
public sealed class MatchStatsManager : SingletonBase<MatchStatsManager>, IMatchEvents, IPlayerEvent
{
    [Sync( SyncFlags.FromHost )] private NetList<PlayerStats> tracked { get; set; } = new();
    // Custom leaderboard data class for team based stats might be needed

	public IEnumerable<PlayerStats> Tracked => [.. tracked];

    [Rpc.Host]
    void IMatchEvents.OnKill( PlayerStats killed, DamageInfo damageInfo )
	{
		if ( killed == null )
		{
			Log.Error( "No killed character given, ignoring." );
			return;
		}

        if ( !damageInfo.Attacker.Components.TryGet<PlayerStats>( out var attacker ) )
		{
			Log.Error( $"DamageInfo for death of {killed} did not contain attacker, ignoring." );
			return;
		}

        if ( killed == attacker )
        {
            Log.Info( "kyssed :D" );
            return; // Killing yourself should not count as a kill
        }

		attacker.AddKill();
		attacker.AddDamage( damageInfo.Damage );
        attacker.AddScore( 1 );
        UpdateScore( 1, "kill" );
        MatchManager.Instance.MatchGameMode.WinCondition( attacker );
    }

    [Rpc.Broadcast( NetFlags.SendImmediate | NetFlags.HostOnly )]
    private void UpdateScore(int amount = 0, string reason = "refresh")
    {
        IMatchEvents.Post( e => e.OnScoreAdded( amount, reason ) );
    }

    // bad but works
    // parempi ratkaisu olisi varmaan muokata pelaaja prefabia niin, että sen parentissa olisi objekteja mitä ei ikinä
    // poisteta (mm. statsit)
    [Rpc.Host]
    public void RemovePreviousStats( PlayerStats prevStats )
    {
        tracked.Remove( prevStats );
        UpdateScore();
    }

    /// <summary>
    /// Allows registering both players and bots/NPCs
    /// </summary>
    /// <param name="character"></param>
    [Rpc.Host]
	public void RegisterCharacter( GameObject character )
	{
		if (character.Components.TryGet<PlayerStats>( out var stats ))
        {
			tracked.Add( stats );
            UpdateScore();
        }
	}

	void IPlayerEvent.OnSpawn( GameObject character )
    {
		RegisterCharacter( character );
	}

    [Rpc.Host]
    void IMatchEvents.OnPlayerLeft(Guid connectionId)
    {
        var toRemove = tracked.FirstOrDefault(p => p.Network.OwnerId == connectionId);
        if (toRemove is not null)
            tracked.Remove(toRemove);
    }

}
