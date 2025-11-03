using Sandbox;

namespace Shooter;

public sealed class PlayerStats : Component, ISerializable
{
	public int Score { get; private set; } = 0;
	public int Kills { get; private set; } = 0;
	public float Damage { get; private set; } = 0;
	public int Deaths { get; private set; } = 0;

	public string Name => "player_stats";
	public bool ShouldAccumulate => true;

	// Assists, damage, favourite gun etc.

	public void AddKill() => Kills++;
	public void AddDamage( float damage ) => Damage += damage; 
	public void AddDeath() => Deaths++;
	public void AddScore( int amount ) => Score += amount;

	public void Accumulate( ISerializable data )
	{
		if ( data == null || data is not PlayerStats stats ) return;

		this.Score += stats.Score;
		this.Kills += stats.Kills;
		this.Damage += stats.Damage;
		this.Deaths += stats.Deaths;
	}
}
