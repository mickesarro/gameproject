using Sandbox;
using Shooter.Camera;
using System;
using System.Diagnostics.Metrics;

namespace Shooter;

/// <summary>
/// Component for adding health and related functionality to a character.
/// Implements IDamageable.
/// </summary>
public sealed class CharacterHealth : Component, Component.IDamageable, IMatchEvents, IPlayerEvent
{
	private PlayerStats ownedStats;

	[Property] public float MaxHealth { get; private set; } = 100f;

    [Description("Should not be manually altered in editor outside testing!")]
	[Property, Sync] public float Health { get; set; } = 100f;

	public bool IsAlive => Health > 0;

    public Action<DamageInfo> OnDamage { get; set; }

    private CharacterRagdoll CharacterRagdoll { get; set; }
    
    private ICharacterBase CharacterBase { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		ownedStats = GetComponent<PlayerStats>();

        CharacterRagdoll = GetComponent<CharacterRagdoll>();

        CharacterBase = GetComponent<ICharacterBase>();
    }

	[Rpc.Owner]
	public void TakeDamage( DamageInfo damageInfo )
	{
		if ( IsProxy ) return;

		Health -= damageInfo.Damage;
		// Log.Info( $"Dealt {damageInfo.Damage} by {damageInfo.Attacker} " );

        // Flinch animations, screen red etc.
        OnDamage?.Invoke( damageInfo );

        if ( Health <= 0 )
		{
            Health = 0;
			Death( damageInfo );
		}
	}

	/// <summary>
	/// Deal damage to character.
	/// </summary>
	/// <param name="damageInfo"></param>
	void IDamageable.OnDamage( in DamageInfo damageInfo )
	{
		TakeDamage( damageInfo );
	}

	public void ReSpawn(float health)
	{
        // Log.Info("ReSpawn");
        // Should probably go through gamemode to determine whether spawning is allowed
        GameObject.GetComponent<CharacterSpawner>( includeDisabled: true )
            .Respawn( Spawner.GetSpawnPoint() );
	}

	/// <summary>
	/// Called on the event of characters death.
	/// </summary>
	//[Rpc.Owner]
	private void Death( DamageInfo damageInfo )
	{
		// Animations

        if ( IsProxy ) return;

        ownedStats.AddDeath();
        
        var velocity = CharacterBase.Velocity;
        if (damageInfo.Tags.Contains( "explosion" )) 
            velocity += ExplosionExtraVelocity( damageInfo );
        
        CreateRagdoll( velocity );
        // And this is used for registering the kill
        IMatchEvents.Post( e => e.OnKill( ownedStats, damageInfo ) );
        // This is used for notifying networked objects        
        BroadcastOnKill( damageInfo );

        // Need to implement respawning etc. while maintaining the same gameobject
        GameObject.Enabled = false;

        if ( GameObject.Tags.Has( "npc" ) ) ReSpawn( 0 );
        else IPlayerEvent.Post( e => e.OnDied( damageInfo ) );
    }

    private void CreateRagdoll(Vector3 velocity)
    {
        if (IsProxy) return;

        var ragdoll = CharacterRagdoll.CreateRagdoll(velocity);
        ragdoll.NetworkSpawn(Network.Owner);

        if (GameObject.Tags.Has("npc")) return;

        var camera = MatchManager.Instance?.MatchGameMode
            ?.Camera.CreateCamera();

        if (camera == null)
        {
            ReSpawn(0);
            return;
        }

        camera.FollowObject = ragdoll;
        camera.position = ragdoll.WorldPosition;
        camera.Enable();
        //gc.Radius = 50.0f;
    }

    private static Vector3 ExplosionExtraVelocity( DamageInfo damageInfo )
    {
        return damageInfo.Position - damageInfo.Origin;
    }
    
    [Rpc.Broadcast( NetFlags.OwnerOnly )]
    private void BroadcastOnKill( DamageInfo damageInfo ) 
        => IMatchEvents.Post( e => e.BroadcastOnKill( ownedStats, damageInfo ) );

}
