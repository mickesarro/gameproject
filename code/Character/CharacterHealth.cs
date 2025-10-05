using Sandbox;
using Sandbox.Utility;

/// <summary>
/// Component for adding health and related functionality to a character.
/// Implements IDamageable.
/// </summary>
public sealed class CharacterHealth : Component, Component.IDamageable
{
	[Property] public float MaxHealth { get; private set; } = 100f;
	[Property, Sync] public float Health { get; private set; } = 100f;
	[Property, Hide, Sync] public int Deaths { get; private set; }

	public bool IsAlive => Health > 0;

	[Rpc.Broadcast]
	public void OnDamage( DamageInfo damage )
	{
		Health -= damage.Damage;
		Log.Info( $"Dealt {damage.Damage} by {damage.Attacker} " );

		// Flinch animations, screen red etc.

		if ( Health <= 0 )
		{
			Death();
		}
	}

	/// <summary>
	/// Deal damage to character.
	/// </summary>
	/// <param name="damage"></param>
	public void OnDamage( in DamageInfo damage ) 
	{
		// This is needed to use Rpc.Broadcast on the proper one
		// as it doesn't allow references
		OnDamage( damage ); 
	}

	public void ReSpawn(float health)
	{
		Health = health;
	}

	/// <summary>
	/// Called on the event of characters death.
	/// </summary>
	[Rpc.Broadcast]
	private void Death()
	{
		// Animations

		++Deaths;

		Log.Info( $"I, {Steam.SteamId.ToString()}, died" );
		DestroyGameObject();
	}
}
