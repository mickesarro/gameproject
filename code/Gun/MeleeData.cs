using Sandbox;

namespace Shooter;

public sealed class MeleeData : Component
{
    [Property, RequireComponent] public GameObject ViewModel { get; private set; }
    [Property] public GameObject Worldmodel { get; private set; }

    [Property] public float Damage { get; private set; } = 25f;
    [Property] public float Range { get; private set; } = 100f;
    [Property] public float HitRadius { get; private set; } = 10f;
    [Property] public float Cooldown { get; private set; } = 1f;
}  
