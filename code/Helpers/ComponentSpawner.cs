using System.Text.Json.Nodes;

namespace Shooter.Helpers;

public sealed class ComponentSpawner : Component
{
    [Property]
    private Component component { get; set; }
    private TypeDescription typeDesc;
    private float timeSinceInvalid;

    protected override void OnStart()
    {
        if ( component == null )
            return;

        typeDesc = TypeLibrary.GetType( component.GetType() );
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (component != null && component.IsValid)
        {
            timeSinceInvalid = 0f;
            return;
        }
        timeSinceInvalid += Time.Delta;
        if (timeSinceInvalid >= 5f)
        {
            Log.Info("Spawning component after 5 seconds...");
            component = Components.Create(typeDesc, false);
            component.Enabled = true;
            timeSinceInvalid = 0f;
        }
    }
}
