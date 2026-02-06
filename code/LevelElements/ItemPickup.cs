using Shooter.Sounds;
namespace Shooter;

/// <summary>
/// Allows items in world to be pickable by running into them
/// </summary>
public sealed class ItemPickup : Component, Component.ITriggerListener
{
	// Imitates the one that was done using the visual script

	[Property] public GameObject ItemPrefab { get; private set; }
	[Property] private float Spin { get; set; } = 0f;

    [Property] private HideForTime hideForTime;

	protected override void OnAwake()
	{
		base.OnAwake();
		if (ItemPrefab == null)
		{
			Log.Error( "No item prefab provided, destroying." );
			DestroyGameObject();
		}
        hideForTime ??= GetOrAddComponent<HideForTime>();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		GameObject.WorldRotation *= Rotation.FromYaw( Spin * Time.Delta );
	}

	/// <summary>
	/// Implemented ITriggerListener method that handles spawning the item collected.
	/// </summary>
	/// <param name="other"></param>
	public void OnTriggerEnter( Collider other )
	{
		
		if ( other.Tags.Contains( "player" ) && !hideForTime.IsHiding() )
		{
			//Log.Info( other );
			if ( !other.IsProxy )
			{
				GameObject Parent = other.GameObject;

				// Not enabling at first is important as picking up new weapons would then result in multiple enabled
				var item = ItemPrefab.Clone( Parent.WorldTransform, parent: Parent, startEnabled: false, name: ItemPrefab.Name );

				var spawnOptions = new NetworkSpawnOptions
				{
					OrphanedMode = NetworkOrphaned.Destroy,
					StartEnabled = item.Enabled,
					Owner = Parent.Network.Owner,
				};
				item.NetworkSpawn( spawnOptions );

				item.GetComponent<ICollectable>( includeDisabled: true )?.Collect( Parent );

				SoundManager.PlayLocal(SoundManager.SoundType.Reload);
			}
            //DestroyGameObject();
            hideForTime.HideFor();
        }
    }

}
