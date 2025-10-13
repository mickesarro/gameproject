using Sandbox;
using Sandbox.Utility;

/// <summary>
/// Component for adding health and related functionality to a character.
/// Implements IDamageable.
/// </summary>
public sealed class CharacterHealth : Component, Component.IDamageable
{
	[Property] public float MaxHealth { get; private set; } = 100f;
	[Sync] public float Health { get; set; } = 100f;
	[Hide, Sync] public int Deaths { get; private set; }

	public bool IsAlive => Health > 0;

	[Rpc.Owner]
	public void OnDamage( float damage, GameObject attacker )
	{
		if ( IsProxy ) return;

		Health -= damage;
		Log.Info( $"Dealt {damage} by {attacker} " );

		// Flinch animations, screen red etc.

		if ( Health <= 0 )
		{
			Death();
		}
	}

	/// <summary>
	/// Deal damage to character.
	/// </summary>
	/// <param name="damageInfo"></param>
	void IDamageable.OnDamage( in DamageInfo damageInfo )
	{
		OnDamage( damageInfo.Damage, damageInfo.Attacker );
	}

	public void ReSpawn(float health)
	{
		Health = health;
	}

	/// <summary>
	/// Called on the event of characters death.
	/// </summary>
	[Rpc.Owner]
	private void Death()
	{
		// Animations

		++Deaths;

		Log.Info( $"I, {Steam.SteamId.ToString()}, died" );
		DestroyGameObject();
	}
}
