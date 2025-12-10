using Sandbox;

namespace Shooter;

public sealed class PlayerStats : Component, ISerializable
{
    [Sync] public int Score { get; private set; } = 0;
    [Sync] public int Kills { get; private set; } = 0;
    [Sync] public float Damage { get; private set; } = 0;
    [Sync] public int Deaths { get; private set; } = 0;
    
    public string Name => "player_stats";
    public bool ShouldAccumulate => true;

    [Rpc.Owner]
    public void AddKill()
    {
        // Log.Info("AddKill");
        Kills++;
        // Log.Info( Kills );
    }
    //[Rpc.Owner]
    public void AddDeath()
    {
        // Log.Info("AddDeath");
        Deaths++;
        // Log.Info( Deaths );
    }
    [Rpc.Owner]
    public void AddDamage(float damage) => Damage += damage;
    [Rpc.Owner]
    public void AddScore(int amount)   => Score += amount;

    public void Accumulate( ISerializable data )
    {
        if ( data == null || data is not PlayerStats stats ) return;
        //Log.Info( "accumulatin'" );   
        this.Score += stats.Score;
        this.Kills += stats.Kills;
        this.Damage += stats.Damage;
        this.Deaths += stats.Deaths;
    }
}
