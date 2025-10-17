using Sandbox;

public struct PlayerStats : ISerializable
{
	public int Kills { get; private set; }
	public int Deaths { get; private set; }

	public string Name => "player_stats";

	// Assists, damage, favourite gun etc.

	public void Add( PlayerStats playerStats )
	{
		// Could implement with reflection if a lot of members arise
		Kills += playerStats.Kills;
		Deaths += playerStats.Deaths;
	}
}
