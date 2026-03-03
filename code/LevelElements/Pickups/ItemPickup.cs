using Sandbox.Movement;
using Shooter.Sounds;
using System;
namespace Shooter;

/// <summary>
/// Allows items in world to be pickable by running into them
/// </summary>
public sealed class ItemPickup : Pickup, Component.ITriggerListener
{
	// Imitates the one that was done using the visual script

	[Property] public override PrefabFile ItemPrefab { get; set; }
	[Property] private float Spin { get; set; } = 0f;
    public override Action<Pickup> Collected { get; set; }

    protected override void OnAwake()
	{
        if ( ItemPrefab == null )
        {
            //Log.Error( "No item prefab provided, destroying." );
            //DestroyGameObject();
            ModelRenderer.Enabled = false;
            return;
        }

        ModelRenderer.Model = Model.Load( ItemPrefab.GetMetadata( "Model" ));

        base.OnAwake();
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

        if ( ItemPrefab == null ) return;
		
		if ( other.Tags.Contains( "player" ) && !HideForTime.IsHiding() )
		{
			//Log.Info( other );
			if ( !other.IsProxy )
			{
				GameObject Parent = other.GameObject.Root;

				// Not enabling at first is important as picking up new weapons would then result in multiple enabled
				var item = GameObject.Clone( ItemPrefab, Parent.WorldTransform, parent: Parent, startEnabled: false, name: ItemPrefab.ResourceName );

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
            //HideForTime.HideFor();
            Collected?.Invoke( this );
        }
    }

}
