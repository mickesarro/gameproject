
/// <summary>
/// Allows items in world to be pickable by running into them
/// </summary>
public sealed class ItemPickup : Component, Component.ITriggerListener
{
	// Imitates the one that was done using the visual script

	[Property] private GameObject Parent { get; set; }
	[Property] private GameObject ItemPrefab { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		if (ItemPrefab == null)
		{
			Log.Error( "No item prefab provided, destroying." );
			DestroyGameObject();
		}
	}

	/// <summary>
	/// Implemented ITriggerListener method that handles spawning the item collected.
	/// </summary>
	/// <param name="other"></param>
	public void OnTriggerEnter( Collider other )
	{
		
		if ( other.Tags.Contains( "player" ) )
		{
			Log.Info( other );
			if ( !other.IsProxy )
			{
				Parent ??= other.GameObject;

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
			}

			DestroyGameObject();
		}
	}
	
}
