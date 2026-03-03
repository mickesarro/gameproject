using System;
using Sandbox;
using System.Linq;

namespace Shooter.UI;

public sealed class ItemFloaty : Component
{    
    [Property] private SpriteRenderer SourceRenderer { get; set; }
	[Property] private PointLight Light { get; set; }
	
    private CameraComponent Camera;
    private float offset;
    
    const float minDistance = 150f;
    const float maxDistance = 400f;
    const float minScale = 3f;
    const float maxScale = 1f;
    
    protected override void OnStart()
    {
        base.OnStart();
        
        Camera = Scene.Camera;

        if (SourceRenderer == null) 
        {
            SourceRenderer = Components.Get<SpriteRenderer>();
        }
    }
    
    protected override void OnUpdate()
	{
		base.OnUpdate();

		bool owned = IsAlreadyOwned();

		if ( SourceRenderer.IsValid() )
		{
			SourceRenderer.Enabled = !owned;
		}

		if ( Light.IsValid() )
		{
			Light.Enabled = !owned;
		}

		if ( owned ) return;
		
		if ( Camera == null || SourceRenderer?.Texture == null ) return;
	}

    private bool IsAlreadyOwned()
    {
        var pickup = Components.GetInParent<ItemPickup>();
        var prefabGun = pickup?.ItemPrefab?.Components.Get<Gun>( true );
        var inventory = PlayerController.Local?.Components.Get<PlayerInventory>();

        if ( prefabGun == null || inventory == null ) return false;

        int slotIndex = (int)prefabGun.GunData.WeaponType;
        var ownedWeapon = inventory.Items.ElementAtOrDefault( slotIndex );

        if ( ownedWeapon is Gun ownedGun && ownedGun.GunData.PrimaryFireData.AmmoLeft > 0 )
        {
            return true;
        }
        return false;
    }
}