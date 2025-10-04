using Sandbox.Utility;

/// <summary>
/// 
/// </summary>
public sealed class ItemPickup : Component, Component.ITriggerListener
{
	// Imitates the one that was done using the visual script

	[Property] private GameObject Camera { get; set; }
	[Property] private GameObject GunPrefab { get; set; }
	[RequireComponent, Hide] private Collider collider { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		// collider = GetComponent<Collider>();
		// collider.OnObjectTriggerEnter += OnTriggerEnter;
	}

	public void OnTriggerEnter( Collider other )
	{
		if ( other.Tags.Contains( Steam.SteamId.ToString() ) )
		{
			var gun = GunPrefab.Clone( new Transform(), parent: Camera, startEnabled: true );

			// This is to get the first version of gun system workin only. Probably some interface or some other means needed.
			gun.GetComponent<Gun>().User = other.GetComponent<PlayerController>();
			DestroyGameObject();
		}
	}
	
}
