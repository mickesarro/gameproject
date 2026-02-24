using Sandbox;
using Sandbox.Citizen;

namespace Shooter;

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
	
	[Property] public GameObject Worldmodel { get; private set; }
	[Property] public CitizenAnimationHelper.HoldTypes holdType { get; private set; } = CitizenAnimationHelper.HoldTypes.None;
    
    [Property] public bool AutomaticReload { get; private set; } = true;
}
