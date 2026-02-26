using Sandbox;

namespace Shooter;

/// <summary>
/// Implement to subscribe to player events.
/// </summary>
public interface IPlayerEvent : ISceneEvent<IPlayerEvent>
{
	void OnTakeDamage( float damage ) { }
	
	void OnDied( DamageInfo damageInfo ) { }
	void OnSpawn( GameObject self ) { }
	void OnWeaponAdded( IWeapon weapon ) { }
	void OnItemAdded( ICollectable item ) { }
	void OnSwitchItem( ICollectable collectable ) { }
	void OnSwitchItem( InventorySlot slot ) { }
}
