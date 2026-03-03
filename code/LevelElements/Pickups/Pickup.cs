namespace Shooter;

public abstract class Pickup : Component
{
    public enum Rarity { Common, Rare };
    [Property] public virtual Rarity PickupRarity { get; set; } = Rarity.Common;

    [Property] public virtual bool ShouldRandomize { get; set; } = false;


    [Property] protected virtual HideForTime HideForTime { get; set; } = null;

    protected override void OnAwake()
    {
        base.OnAwake();

        HideForTime ??= GetOrAddComponent<HideForTime>();
    }

}
