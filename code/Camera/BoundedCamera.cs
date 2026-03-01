using Sandbox;

namespace Shooter.Camera;

public enum CameraType { Orbit, Flying }; // Add follow etc. when created

public static class CameraTypeExtensions
{
    public static BoundedCamera CreateCamera( this CameraType type )
    {
        GameObject go = new();
        BoundedCamera boundedCamera = null;
        switch ( type )
        {
            case CameraType.Orbit:
                boundedCamera = go.AddComponent<OrbitCamera>();
                break;
            case CameraType.Flying:
                boundedCamera = go.AddComponent<FlyingCamera>();
                break;
        }
        ;
        return boundedCamera;
    }
}

/// <summary>
/// A general base class for different controllable camera implementations.
/// </summary>
public abstract class BoundedCamera : Component
{
    /// <summary>
    /// The position to work around
    /// </summary>
    public Vector3 position = Vector3.Zero;

    /// <summary>
    /// The gameobject the camera can follow
    /// </summary>
    public GameObject FollowObject = null;

    /// <summary>
    /// Rotation cap for limitied views
    /// </summary>
    public float rotationCap = 0.0f;

    /// <summary>
    /// Camera rotation and/or movement speed
    /// </summary>
    public float speed = 2.0f;

    protected CameraComponent camera;

    // Internals related to moving
    protected Angles LookAngle = Angles.Zero;
    protected Vector3 Direction = Vector3.Zero;

    protected override void OnAwake()
    {
        if ( IsProxy )
        {
            Destroy();
            return;
        }
        base.OnAwake();
        Disable();
    }

    protected override void OnEnabled()
    {

        base.OnEnabled();

        camera ??= Scene.Camera;

        if ( position == Vector3.Zero && FollowObject != null )
            position = FollowObject.WorldPosition;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        GatherInput();
        Move();
        Rotate();
    }

    /// <summary>
    /// Should gather the required look angles, movement input etc.
    /// </summary>
    protected abstract void GatherInput();

    /// <summary>
    /// Should move the camera component
    /// </summary>
    protected virtual void Move()
    {
        camera.WorldPosition += Direction * speed;
    }

    /// <summary>
    /// Should handle the rotation of the camera
    /// </summary>
    protected virtual void Rotate()
    {
        camera.WorldRotation = LookAngle.ToRotation();
    }

    public virtual void Enable()
    {
        GameObject.Enabled = true;
    }

    public virtual void Disable()
    {
        GameObject.Enabled = false;
    }
}
