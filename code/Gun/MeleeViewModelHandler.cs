using System;

namespace Shooter;

public sealed class MeleeViewModelHandler : Component
{
    private CameraComponent camera;

    protected override void OnStart()
    {
        base.OnStart();
        camera = Scene.Camera.Components.Get<CameraComponent>();
        if (camera == null)
        {
            Destroy();
            return;
        }

        if (!GetComponentInParent<MeleeWeapon>().User.Components.TryGet<PlayerController>(out _))
        {
            DestroyGameObject();
        }
    }

    protected override void OnUpdate()
    {
        if (IsProxy) return;
        GameObject.WorldPosition = camera.WorldPosition;
        GameObject.WorldRotation = camera.WorldRotation;
    }
}
