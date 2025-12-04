using System;
using Sandbox;

namespace Shooter;

/// <summary>
/// Central authority on logging match stats
/// </summary>
public sealed class MatchStatsManager : SingletonBase<MatchStatsManager>, IMatchEvents, IPlayerEvent
{
	[Sync] private NetList<PlayerStats> tracked { get; set; } = new();

	public IEnumerable<PlayerStats> Tracked => [.. tracked];

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
        killed.AddDeath();
	}

	/// <summary>
	/// Allows registering both players and bots/NPCs
	/// </summary>
	/// <param name="character"></param>
	public void RegisterCharacter( GameObject character )
    {
        if (character.Components.TryGet<PlayerStats>( out var stats ))
		{
			tracked.Add( stats );
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
        foreach ( var stats in Tracked )
        {
            Log.Info( stats.Id + ": " + stats.Kills );            
        }
    }
}
