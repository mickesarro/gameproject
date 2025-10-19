using Sandbox;

/// <summary>
/// Central authority on logging match stats
/// </summary>
public sealed class MatchStatsManager : SingletonBase<MatchStatsManager>, IMatchEvents, IPlayerEvent
{
	[Sync] private NetList<GameObject> tracked { get; set; } = new();

	public IEnumerable<GameObject> Tracked => [.. tracked];

	void IMatchEvents.OnKill( ICharacterBase killed, DamageInfo damageInfo )
	{
		if ( killed == null )
		{
			Log.Error( "No killed character given" );
			return;
		}

		if ( !damageInfo.Attacker.Components.TryGet<ICharacterBase>( out var attacker ) )
		{
			Log.Error( "DamageInfo did not contain attacker" );
			return;
		}

		killed.CharacterStats.AddDeath();

		attacker.CharacterStats.AddKill();
		attacker.CharacterStats.AddDamage( damageInfo.Damage );
	}

	/// <summary>
	/// Allows registering both players and bots/NPCs
	/// </summary>
	/// <param name="character"></param>
	public void RegisterCharacter( GameObject character )
	{
		Log.Info( "Here" );
		if (character.Components.TryGet<ICharacterBase>( out _ ))
		{
			Log.Info( "Here" );
			tracked.Add( character );
		}
	}

	void IPlayerEvent.OnSpawn( GameObject character )
	{
		RegisterCharacter( character );
	}

}
