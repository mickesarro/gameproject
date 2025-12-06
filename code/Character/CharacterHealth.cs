using Sandbox;
using Sandbox.Utility;
using System;

namespace Shooter;

/// <summary>
/// Component for adding health and related functionality to a character.
/// Implements IDamageable.
/// </summary>
public sealed class CharacterHealth : Component, Component.IDamageable, IMatchEvents
{
	private PlayerStats ownedStats;

	[Property] public float MaxHealth { get; private set; } = 100f;

    [Description("Should not be manually altered in editor outside testing!")]
	[Property, Sync] public float Health { get; set; } = 100f;
	[Hide, Sync] public int Deaths { get; private set; }

	public bool IsAlive => Health > 0;

    public Action<DamageInfo> OnDamage { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		ownedStats = GetComponent<PlayerStats>();
	}

	[Rpc.Owner]
	public void TakeDamage( DamageInfo damageInfo )
	{
		if ( IsProxy ) return;

		Health -= damageInfo.Damage;
		Log.Info( $"Dealt {damageInfo.Damage} by {damageInfo.Attacker} " );

        // Flinch animations, screen red etc.
        OnDamage?.Invoke( damageInfo );

		if ( Health <= 0 )
		{
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
        // Should probably go through gamemode to determine whether spawning is allowed
        GameObject.GetComponent<CharacterSpawner>( includeDisabled: true )
            .Respawn( MatchManager.Instance.MatchGameMode.GetSpawnPoint() );
	}

	/// <summary>
	/// Called on the event of characters death.
	/// </summary>
	[Rpc.Owner]
	private void Death( DamageInfo damageInfo )
	{
		// Animations

		++Deaths;

		IMatchEvents.Post( e => e.OnKill( ownedStats, damageInfo ) );

		Log.Info( $"I, {Steam.SteamId.ToString()}, died" );

		// Need to implement respawning etc. while maintaining the same gameobject
		GameObject.Enabled = false;
        ReSpawn( 0 );
	}
}
