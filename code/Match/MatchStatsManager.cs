using Sandbox;

namespace Shooter;

/// <summary>
/// Central authority on logging match stats
/// </summary>
public sealed class MatchStatsManager : SingletonBase<MatchStatsManager>, IMatchEvents, IPlayerEvent
{
    // Custom leaderboard data class for team based stats might be needed
	[Sync] private NetList<GameObject> tracked { get; set; } = new();
    public ICharacterBase Top { get; private set; } = null;

	public IEnumerable<GameObject> Tracked => [.. tracked];

	void IMatchEvents.OnKill( ICharacterBase killed, DamageInfo damageInfo )
	{
		if ( killed == null )
		{
			Log.Error( "No killed character given, ignoring." );
			return;
		}

		if ( !damageInfo.Attacker.Components.TryGet<ICharacterBase>( out var attacker ) )
		{
			Log.Error( $"DamageInfo for death of {killed} did not contain attacker, ignoring." );
			return;
		}

		killed.CharacterStats.AddDeath();

		if ( killed == attacker ) return; // Killing yourself should not count as a kill

		attacker.CharacterStats.AddKill();
		attacker.CharacterStats.AddDamage( damageInfo.Damage );

        attacker.CharacterStats.AddScore( 100 ); // Define score amounts somewhere
        IMatchEvents.Post( e => e.OnScore( 100 ) );
        UpdateTop( attacker );
	}

	/// <summary>
	/// Allows registering both players and bots/NPCs
	/// </summary>
	/// <param name="character"></param>
	public void RegisterCharacter( GameObject character )
	{
		if (character.Components.TryGet<ICharacterBase>( out var characterBase ))
		{
			tracked.Add( character );

            Top ??= characterBase;
        }
	}

    private void UpdateTop( ICharacterBase updated )
    {
        if (updated.CharacterStats.Score > (Top?.CharacterStats.Score ?? -1))
        {
            Top = updated;
        }
    }

	void IPlayerEvent.OnSpawn( GameObject character )
	{
		RegisterCharacter( character );
	}

}
