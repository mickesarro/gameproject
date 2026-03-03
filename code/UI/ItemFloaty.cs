using System;
using Sandbox;
using System.Linq;

namespace Shooter.UI;

public sealed class ItemFloaty : Component
{
    [Property] private Color SpriteColor { get; set; } = Color.White;
    [Property] private float Size { get; set; } = 64f;
    
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
        
        offset = Size / 2;
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

		var pos = Camera.PointToScreenPixels(GameObject.WorldPosition);
		var cameraForward = Camera.WorldTransform.Rotation.Forward;
		var directionToItem = (GameObject.WorldPosition - Camera.WorldTransform.Position).Normal;
		var dotProduct = Vector3.Dot(cameraForward, directionToItem);
		
		if (dotProduct > 0)
		{
			var scale = Scale();
			var sizeWithScale = Size * scale;
			var offsetWithScale = sizeWithScale / 2;

			Camera.Hud.DrawTexture(
				SourceRenderer.Texture, 
				new Rect(pos.x - offsetWithScale, pos.y - offsetWithScale, sizeWithScale, sizeWithScale), 
				SpriteColor
			);
		}
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

    float Scale()
    {
        var distance = GameObject.WorldPosition.Distance(Camera.WorldPosition);
        var t = (distance - minDistance) / (maxDistance - minDistance);
        t = t.Clamp(0f, 1f);
        return MathX.Lerp(minScale, maxScale, t);
    }
}