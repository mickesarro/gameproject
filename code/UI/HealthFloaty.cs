using System;
using Sandbox;
using System.Linq;

namespace Shooter.UI;

public sealed class HealthFloaty : Component
{    
    [Property] private SpriteRenderer SourceRenderer { get; set; }
    [Property] private PointLight Light { get; set; }


    protected override void OnStart()
    {
        base.OnStart();

        if (SourceRenderer == null) 
        {
            SourceRenderer = Components.Get<SpriteRenderer>();
        }
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();

        bool isHiding = IsHiding();

        if ( Light.IsValid() )
        {
            Light.Enabled = !isHiding;
        }

        if ( SourceRenderer.IsValid() )
        {
            SourceRenderer.Enabled = !isHiding;
        }
    }

    private bool IsHiding()
    {
        var hideComp = Components.GetInParent<HideForTime>();
        return hideComp != null && hideComp.IsHiding();
    }
}