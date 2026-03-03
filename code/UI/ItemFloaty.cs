using System;
using Sandbox;
using System.Linq;

namespace Shooter.UI;

public sealed class ItemFloaty : Component
{    
    [Property] private SpriteRenderer SourceRenderer { get; set; }
    [Property] private PointLight Light { get; set; }

    /// <summary>
    /// If true, the floaty and light will be completely disabled if the player already owns this weapon.
    /// </summary>
    [Property] public bool OnlyShowWhenEmpty { get; set; } = false;
    
    protected override void OnStart()
    {
        base.OnStart();

        if (SourceRenderer == null) 
        {
            SourceRenderer = Components.Get<SpriteRenderer>();
        }
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();

        bool isOwned = IsAlreadyOwned();
        bool isHiding = IsHiding();


        // New Logic: 
        // 1. Hide if the "HideForTime" component says so.
        // 2. Hide if OnlyShowWhenEmpty is on AND we already have the weapon.
        bool shouldShow = !isHiding;

        if ( OnlyShowWhenEmpty && isOwned )
        {
            shouldShow = false;
        }

        


        // Apply visibility to Light
        if ( Light.IsValid() )
        {
            Light.Enabled = shouldShow;
        }

        // Apply visibility and Overlay to Renderer
        if ( SourceRenderer.IsValid() )
        {
            SourceRenderer.Enabled = shouldShow;

            if ( shouldShow ) 
            {
                // Overlay (see through walls) is active only if NOT owned
                SourceRenderer.RenderOptions.Overlay = !isOwned;
            }
        }

        if ( !shouldShow ) return;
        
        // Add any rotation or bobbing logic here...
    }

    private bool IsHiding()
    {
        var hideComp = Components.GetInParent<HideForTime>();
        return hideComp != null && hideComp.IsHiding();
    }

    private bool IsAlreadyOwned()
    {
        var pickup = Components.GetInParent<ItemPickup>();
        var prefabGun = pickup?.ItemPrefab?.Components.Get<Gun>( true );
        var inventory = PlayerController.Local?.Components.Get<PlayerInventory>();

        if ( prefabGun == null || inventory == null ) return false;

        int slotIndex = (int)prefabGun.GunData.WeaponType;
        var ownedWeapon = inventory.Items.ElementAtOrDefault( slotIndex );

        // Returns true if the player has the gun and it has ammo
        return ownedWeapon is Gun { GunData.PrimaryFireData.AmmoLeft: > 0 };
    }
}