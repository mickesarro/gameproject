using System;

namespace Shooter;

/// <summary>
/// Handles the movement of the gun.
/// </summary>
public sealed class GunViewModelHandler : Component
{
    protected override void OnStart()
	{
        if (IsProxy) return;
		base.OnStart();
        // NPC does not need viewmodel
        // tägit jostain syystä luotettavempia kun Gunin isPlayer
        // jos sitä käyttää, niin komponentti suoritettiin kaks kertaa per npc,
        // minkä ansiosta DestroyGameObject() heitää npe:n :)
        if ( Tags.Has( "npc" ) )
        {
            var arms = GameObject.Parent?.Children?.Find(o => o.Name == "arms");
            arms?.Destroy();
            DestroyGameObject();
            return;
        }
        GameObject.Parent = Scene.Camera.GameObject;
    }
}
