using Sandbox;
using Sandbox.Citizen;

namespace Shooter;

/// <summary>
/// Holds the data specific to the gun agnostic of the firing data.
/// </summary>
public sealed class GunData : Component, ISceneMetadata
{
	[Property] public Texture Icon { get; private set; }
	[Property] public WeaponType WeaponType { get; private set; } = WeaponType.Primary;
    [Property] public WeaponRarity Rarity { get; private set; } = WeaponRarity.Common;
    [Property] public Model Model { get; private set; }

    // Might be better to move this to gun itself
    [Property, RequireComponent] public FireData PrimaryFireData { get; private set; }
	[Property] public GameObject BarrelEnd { get; private set; } // Spawn point

	[Property] public GameObject Viewmodel { get; private set; }
	
	[Property] public GameObject Worldmodel { get; private set; }
	[Property] public CitizenAnimationHelper.HoldTypes holdType { get; private set; } = CitizenAnimationHelper.HoldTypes.None;
    
    [Property] public bool AutomaticReload { get; private set; } = true;

    public Dictionary<string, string> GetMetadata()
    {
        return new()
        {
            {"Rarity", Rarity.ToString() },
            {"WeaponType", WeaponType.ToString() },
            {"Icon", Icon.ResourcePath },
            {"Model", Model.ResourcePath }
        };
    }
}
