using Sandbox;

public sealed class PlayerStats : Component, ISerializable
{
	public int Score { get; private set; } = 0;
	public int Kills { get; private set; } = 0;
	public float Damage { get; private set; } = 0;
	public int Deaths { get; private set; } = 0;

	public string Name => "player_stats";

	// Assists, damage, favourite gun etc.

	public void AddKill() => Kills++;
	public void AddDamage( float damage ) => Damage += damage; 
	public void AddDeath() => Deaths++;
	public void AddScore( int amount ) => Score += amount;

}
