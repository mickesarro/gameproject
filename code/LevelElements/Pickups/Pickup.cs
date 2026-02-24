namespace Shooter;

public abstract class Pickup : Component
{
    public virtual bool ShouldRandomize { get; set; } = false;

    [Property] protected virtual HideForTime HideForTime { get; set; } = null;

    protected override void OnAwake()
    {
        base.OnAwake();

        HideForTime ??= GetOrAddComponent<HideForTime>();
    }

}
