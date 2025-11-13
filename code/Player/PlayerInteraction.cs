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

	    if (PlayerInventory != null)
		{
			var meleePrefab = GameObject.GetPrefab("assets/melee.prefab");
			if (meleePrefab != null)
			{
				var meleeInstance = meleePrefab.Clone();
				meleeInstance.Name = "Melee";
				meleeInstance.Parent = GameObject;
				meleeInstance.Enabled = true;

				var meleeWeapon = meleeInstance.Components.Get<MeleeWeapon>();
				if (meleeWeapon != null)
				{
					meleeWeapon.User = GameObject;
					PlayerInventory.AddItem(meleeWeapon);
				}
			}
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
