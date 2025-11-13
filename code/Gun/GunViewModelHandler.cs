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
		if (IsProxy) return;
		base.OnStart();
        camera = Scene.Camera;
		if (camera == null )
		{
			Log.Info( "No camera found, destroying." );
			Destroy();
		}

        // NPC does not need viewmodel
        if ( !GetComponentInParent<Gun>().IsPlayer )
        {
            DestroyGameObject();
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
