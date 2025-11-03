using Sandbox;
using Sandbox.Utility;

namespace Shooter;

/// <summary>
/// Component for adding health and related functionality to a character.
/// Implements IDamageable.
/// </summary>
public sealed class CharacterHealth : Component, Component.IDamageable, IMatchEvents
{
	private ICharacterBase ownerCharacter;

	[Property] public float MaxHealth { get; private set; } = 100f;
	[Sync] public float Health { get; set; } = 100f;
	[Hide, Sync] public int Deaths { get; private set; }

	public bool IsAlive => Health > 0;

	protected override void OnAwake()
	{
		base.OnAwake();

		ownerCharacter = GetComponent<ICharacterBase>();
	}

	[Rpc.Owner]
	public void TakeDamage( DamageInfo damageInfo )
	{
		if ( IsProxy ) return;

		Health -= damageInfo.Damage;
		Log.Info( $"Dealt {damageInfo.Damage} by {damageInfo.Attacker} " );

		// Flinch animations, screen red etc.

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
		Health = health;
	}

	/// <summary>
	/// Called on the event of characters death.
	/// </summary>
	[Rpc.Owner]
	private void Death( DamageInfo damageInfo )
	{
		// Animations

		++Deaths;

		IMatchEvents.Post( e => e.OnKill( ownerCharacter, damageInfo ) );

		Log.Info( $"I, {Steam.SteamId.ToString()}, died" );

		// Need to implement respawning etc. while maintaining the same gameobject
		GameObject.Enabled = false;
	}
}
