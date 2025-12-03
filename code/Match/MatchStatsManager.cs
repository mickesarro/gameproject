using Sandbox;

namespace Shooter;

/// <summary>
/// Central authority on logging match stats
/// </summary>
public sealed class MatchStatsManager : SingletonBase<MatchStatsManager>, IMatchEvents, IPlayerEvent
{
	//[Sync] private NetList<PlayerStats> tracked { get; set; } = new();

	//public IEnumerable<PlayerStats> Tracked => [.. tracked];

	void IMatchEvents.OnKill( PlayerController killed, DamageInfo damageInfo )
	{
        Log.Info( "OnKill" );
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
        Log.Info("ATTACKER ID: " + attacker.Id );

        //if ( killed == attacker ) return; // Killing yourself should not count as a kill
        
        Log.Info( "KILLED" );
        attacker.AddKill();
        attacker.AddDamage( damageInfo.Damage );
        killed.CharacterStats.AddDeath();
	}

	/// <summary>
	/// Allows registering both players and bots/NPCs
	/// </summary>
	/// <param name="character"></param>
	public void RegisterCharacter( GameObject character )
    {
        if (character.Components.TryGet<PlayerStats>( out var stats ))
		{
			//tracked.Add( stats );
		}
        
    }

	void IPlayerEvent.OnSpawn( GameObject character )
    {
        character.Components.TryGet<PlayerStats>( out var stats );
        Log.Info( GameObject.Id );
		RegisterCharacter( character );
	}
    
    protected override void OnUpdate()
    {
        base.OnUpdate();

    }
}
