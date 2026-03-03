using System;

namespace Shooter;

public abstract class Pickup : Component
{
    [Property] public virtual PrefabFile ItemPrefab { get; set; }
    [Property, RequireComponent] public virtual SkinnedModelRenderer ModelRenderer { get; set; }

    public enum Rarity { Common, Rare };
    [Property] public virtual Rarity PickupRarity { get; set; } = Rarity.Common;

    [Property] public virtual bool ShouldRandomize { get; set; } = false;


    [Property] protected virtual HideForTime HideForTime { get; set; } = null;

    public abstract Action<Pickup> Collected { get; set; }

    protected override void OnAwake()
    {
        base.OnAwake();

        HideForTime ??= GetOrAddComponent<HideForTime>();
    }

}
