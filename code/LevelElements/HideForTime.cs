using System;
using System.Threading.Tasks;

namespace Shooter;

/// <summary>
/// Can be used to hide GameObjects for a period of time.
/// </summary>
public sealed class HideForTime : Component
{
    [Property] private SkinnedModelRenderer displayRenderer;
    
    [Description( "Time to hide. Set to 0 to hide permanently until restart." )]
    [Property] private float HideTime { get; set; } = 10f;

    [Description( "If you set randomize, set the randomRange" )]
    [Property] private bool randomize = false;
    [Property] private RangedFloat randomRange;

    // Track the active hide task
    private int _hideToken = 0; 

    public bool IsHiding() => !displayRenderer.Enabled;

    protected override void OnAwake()
    {
        base.OnAwake();
        displayRenderer ??= GetComponent<SkinnedModelRenderer>();
    }

    protected override void OnEnabled()
    {
        base.OnEnabled();
        
        _hideToken++;
        
        if ( displayRenderer.IsValid() )
        {
            displayRenderer.Enabled = true;
        }
    }

    public async void HideFor()
    {
        if ( displayRenderer == null ) return;

        if ( randomize )
        {
            HideFor( displayRenderer, randomRange.GetValue() );
            return;
        }

        HideFor( displayRenderer, HideTime );
    }

    public async void HideFor(SkinnedModelRenderer renderer, float time)
    {
        if ( renderer == null ) return;

        renderer.Enabled = false;

        // Hide forever
        if ( time <= 0 ) return; 

        int currentToken = ++_hideToken;

        await Task.DelaySeconds( time );

        if ( currentToken == _hideToken && renderer.IsValid() )
        {
            renderer.Enabled = true;
        }
    }
}