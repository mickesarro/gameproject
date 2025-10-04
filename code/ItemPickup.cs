using Sandbox.UI;
using Sandbox.Utility;

/// <summary>
/// 
/// </summary>
public sealed class ItemPickup : Component
{
	// Imitates the one that was done using the visual script

	[Property] private GameObject Camera { get; set; }
	[Property] private GameObject GunPrefab { get; set; }
	[RequireComponent, Hide] private Collider collider { get; set; }

	protected override void OnAwake()
	{
		base.OnAwake();
		collider = GetComponent<Collider>();
		collider.OnObjectTriggerEnter += OnTriggerEnter;
	}

	private void OnTriggerEnter(GameObject other)
	{
		if ( other.Tags.Contains( Steam.SteamId.ToString() ) )
		{
			var gun = GunPrefab.Clone( new Transform(), parent: Camera, startEnabled: true );

			// This is for beginning testing only. Probably some interface or some other means needed.
			gun.GetComponent<Gun>().User = other.GetComponent<PlayerController>();
			DestroyGameObject();
		}
	}
	
}
