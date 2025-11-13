using Sandbox;
using Sandbox.Citizen;

namespace Shooter;

public sealed class MeleeData : Component
{
    [Property] public float Damage { get; private set; } = 25f;
    [Property] public float Range { get; private set; } = 100f;
    [Property] public float HitRadius { get; private set; } = 10f;
    [Property] public float Cooldown { get; private set; } = 1f;
} 