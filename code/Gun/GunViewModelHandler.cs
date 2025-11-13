using System;

namespace Shooter;

/// <summary>
/// Handles the movement of the gun.
/// </summary>
public sealed class GunViewModelHandler : Component
{
	private CameraComponent camera;
	protected override void OnStart()
	{
        // NPC does not need viewmodel
        // e: this deletes the viewmodel on the player, not dummy
        if ( !GetComponentInParent<Gun>().IsPlayer )
        {
            DestroyGameObject();
        }
        
		if (IsProxy) return;
		base.OnStart();
        camera = Scene.Camera;
		if (camera == null )
		{
			Log.Info( "No camera found, destroying." );
			Destroy();
		}
	}

	protected override void OnUpdate()
	{
        if (IsProxy) return;
		// This is not ideal and must be made independent later.
		GameObject.WorldPosition = camera.WorldPosition;
		GameObject.WorldRotation = camera.WorldRotation;
	}
}
