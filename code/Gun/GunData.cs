using Sandbox;



/// <summary>
/// Holds the data specific to the gun agnostic of the firing data.
/// </summary>
public sealed class GunData : Component
{
	[Property] public WeaponType WeaponType { get; private set; } = WeaponType.Primary;

	// Might be better to move this to gun itself
	[Property, RequireComponent] public FireData PrimaryFireData { get; private set; }
	[Property] public GameObject BarrelEnd { get; private set; } // Spawn point

	[Property] public GameObject Viewmodel { get; private set; }
}
