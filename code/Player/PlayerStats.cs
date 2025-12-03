using Sandbox;

namespace Shooter;

public sealed class PlayerStats : Component
{
    [Sync] public int Score { get; private set; } = 0;
    [Sync] public int Kills { get; private set; } = 0;
    [Sync] public float Damage { get; private set; } = 0;
    [Sync] public int Deaths { get; private set; } = 0;

    [Rpc.Host]
    public void AddKill()
    {
        Log.Info("AddKill");
        Kills++;
        Log.Info( Kills );
    }

    public void AddDeath()   => Deaths++;
    public void AddDamage(float damage) => Damage += damage;
    public void AddScore(int amount)   => Score += amount;
    
}
