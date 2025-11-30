using Sandbox;

namespace Shooter;

/// <summary>
/// Acts as a proxy for different player interactions.
/// </summary>
public sealed class PlayerInteraction : Component, IPlayerEvent
{
	[Property, RequireComponent] private PlayerController Player { get; set; }
	[Property, RequireComponent] private PlayerInventory PlayerInventory { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerInventory = GetComponent<PlayerInventory>();
	}

	protected override void OnStart()
	{
		base.OnStart();
		// Perhaps should move this to some other place
		// Needs to be called on start instead of awake
		IPlayerEvent.Post( e => e.OnSpawn( GameObject ) );

        if ( PlayerInventory == null ) return;

        var meleePrefab = GameObject.Clone( "melee.prefab", new CloneConfig { Name = "Melee", Parent = GameObject, StartEnabled = true} );
        Log.Info( meleePrefab );
        if ( meleePrefab != null && meleePrefab.Components.TryGet<IWeapon>( out var meleeWeapon ) )
        {
            meleeWeapon.User = GameObject;
            PlayerInventory.AddItem( (ICollectable) meleeWeapon );
        }

	}

	void IPlayerEvent.OnSwitchItem( ICollectable collectable )
	{
		// Change animations etc.
	}

	void IPlayerEvent.OnItemAdded( ICollectable item )
	{
		//AddToInventory( item );
	}

	private void AddToInventory( ICollectable collectable )
	{
		// Might not be needed
	}

}
