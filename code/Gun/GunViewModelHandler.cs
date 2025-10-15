using System;

namespace Sandbox;

/// <summary>
/// Handles the movement of the gun.
/// </summary>
public sealed class GunViewModelHandler : Component
{
	private CameraComponent camera;
	protected override void OnAwake()
	{
		base.OnAwake();
		camera = Scene.Camera.Components.Get<CameraComponent>();
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
