using System;

namespace Shooter.UI;

public sealed class ItemFloaty : Component
{
	[Property] private Texture Texture { get; set; }
	[Property] private Color SpriteColor { get; set; }
	[Property] private float Size { get; set; }
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
    }
    
	protected override void OnUpdate()
	{
		base.OnUpdate();
		
		var pos = Camera.PointToScreenPixels(GameObject.WorldPosition);
		
		// AI SLOP
		// muuten piirtä vaikka kamera osottaa päinvastaseen suuntaan

		// Check if the object is in front of the camera
		var cameraForward = Camera.WorldTransform.Rotation.Forward; // The camera's forward direction
		var directionToItem = (GameObject.WorldPosition - Camera.WorldTransform.Position).Normal;

		// Use dot product to check if the item is in front of the camera
		var dotProduct = Vector3.Dot(cameraForward, directionToItem);
		
		// If the dot product is positive, the item is in front of the camera

		if (dotProduct > 0)
		{
            var scale = Scale();
            var sizeWithScale = Size * scale;
			var offsetWithScale = offset * scale;
			Camera.Hud.DrawTexture(Texture, new Rect(pos.x - offsetWithScale, pos.y - offsetWithScale, sizeWithScale, sizeWithScale), SpriteColor);
		}
	}

	float Scale()
	{
		var distance = GameObject.WorldPosition.Distance(Camera.WorldPosition);
		var t = (distance - minDistance) / (maxDistance - minDistance);
		t = t.Clamp(0f, 1f);
		return MathX.Lerp(minScale, maxScale, t);
	}
}
