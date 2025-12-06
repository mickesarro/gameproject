using System;
using Sandbox;

namespace Shooter;

/// <summary>
/// Central authority on logging match stats
/// </summary>
public sealed class MatchStatsManager : SingletonBase<MatchStatsManager>, IMatchEvents, IPlayerEvent
{
	[Sync] private NetList<PlayerStats> tracked { get; set; } = new();
    // Custom leaderboard data class for team based stats might be needed
    public PlayerStats Top { get; private set; } = null;

	public IEnumerable<PlayerStats> Tracked => [.. tracked];

    void IMatchEvents.OnKill( PlayerStats killed, DamageInfo damageInfo )
	{
        Log.Info( "onkill start" );
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
        attacker.AddScore( 100 ); // Define score amounts somewhere
        UpdateTop( attacker );
        Log.Info( "onkill end" );
    }

	/// <summary>
	/// Allows registering both players and bots/NPCs
	/// </summary>
	/// <param name="character"></param>
	public void RegisterCharacter( GameObject character )
	{
		if (character.Components.TryGet<PlayerStats>( out var stats ))
		{
            // bad but removes previous stats, since new ones are always created
            var toRemove = tracked.FirstOrDefault(p => p.Network.OwnerId == stats.Network.OwnerId);
            if (toRemove is not null)
                tracked.Remove(toRemove);
            
			tracked.Add( stats );

            Top ??= stats;
        }
	}

    private void UpdateTop( PlayerStats updated )
    {
        if (updated.Score > (Top?.Score ?? -1))
        {
            Top = updated;
        }
    }

	void IPlayerEvent.OnSpawn( GameObject character )
    {
		RegisterCharacter( character );
	}

    void IMatchEvents.OnPlayerLeft(Guid connectionId)
    {
        var toRemove = tracked.FirstOrDefault(p => p.Network.OwnerId == connectionId);
        if (toRemove is not null)
            tracked.Remove(toRemove);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        foreach ( var stats in tracked )
        {
            //Log.Info( Game.ActiveScene.Get<MatchManager>().MatchGameMode );
            //Log.Info( stats.Id + ": Kills: " + stats.Kills + " Deaths: " + stats.Deaths );
        }
    }
}
