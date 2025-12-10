using System;

namespace Shooter;

/// <summary>
/// Can be used to hide GameObjects for a period of time.
/// </summary>
public sealed class HideForTime : Component
{
    [Property] private SkinnedModelRenderer displayRenderer;
    [Property] private float HideTime { get; set; } = 10f;

    [Description( "If you set randomize, set the randomRange" )]
    [Property] private bool randomize = false;
    [Property] private RangedFloat randomRange;

    public bool IsHiding() => !displayRenderer.Enabled;

    protected override void OnAwake()
    {
        base.OnAwake();

        displayRenderer ??= GetComponent<SkinnedModelRenderer>();
    }

    public async void HideFor()
    {
        if ( displayRenderer == null ) return;

        if ( randomize ) // Simply call the other with random time
        {
            HideFor( displayRenderer, randomRange.GetValue() );
            return;
        }

        displayRenderer.Enabled = false;

        await Task.DelaySeconds( HideTime );

        displayRenderer.Enabled = true;
    }

    public async void HideFor(SkinnedModelRenderer renderer, float time)
    {
        if ( renderer == null ) return;

        renderer.Enabled = false;

        await Task.DelaySeconds( time );

        renderer.Enabled = true;
    }
}
