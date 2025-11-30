using Sandbox;

namespace Shooter;

/// <summary>
/// Simple melee weapon.
/// Inherits from Gun for inventory compatibility, but ignores GunData.
/// </summary>
public sealed class MeleeWeapon : Gun
{
    [Property, RequireComponent] private MeleeData meleeData { get; set; }

    public override WeaponType WeaponType => WeaponType.Melee;
    public override MeleeData MeleeData => meleeData;
    public override GunData GunData => null;

    private TimeSince timeSinceLastAttack;

    public override void Shoot() => Attack();

    /// <summary>
    /// Performs melee attack and returns true if it hit a target.
    /// </summary>
    public bool Attack()
    {
        if (timeSinceLastAttack < (meleeData?.Cooldown ?? 0.5f))
            return false;

        timeSinceLastAttack = 0;

        if (User == null) return false;

        // Trace start and direction
        var startPos = IsPlayer ? Game.ActiveScene.Camera.WorldPosition : User.WorldTransform.Position + Vector3.Up * 50f;
        var forward  = IsPlayer ? Game.ActiveScene.Camera.WorldRotation.Forward : User.WorldTransform.Forward;
        var endPos   = startPos + forward * (meleeData?.Range ?? 100f);

        var trace = Game.ActiveScene.Trace
            .Ray(startPos, endPos)
            .UseHitboxes(true)
            .Size(meleeData?.HitRadius ?? 30f)
            .IgnoreGameObjectHierarchy(User)
            .Run();

        bool hit = false;
        if (trace.Hit && trace.GameObject.GetComponent<IDamageable>() is IDamageable target)
        {
            var damageInfo = new DamageInfo
            {
                Damage = meleeData.Damage,
                Attacker = User,
                Position = trace.HitPosition
            };
            target.OnDamage(damageInfo);
            hit = true;
        }

        PlaySwingAnimation();
        return hit;
    }

    /// <summary>
    /// Plays swing animation on both viewmodel and world model.
    /// </summary>
    private void PlaySwingAnimation()
    {
        // Viewmodel (arms)
        if (viewModelRenderer != null)
            SetAnimation(modelType.ViewModel, "b_attack", true);

    }

    protected override void OnUpdate()
    {
        if (IsProxy || User == null) return;

        if (Input.Pressed("attack1"))
        {
            if (Attack())
                Log.Info("Hit the target!");
            else
                Log.Info("Missed!");
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        if (User != null)
        {
            var viewmodelObj = User.Children.Find(x => x.Name == "viewmodel");
            viewModelRenderer = viewmodelObj?.Components.Get<SkinnedModelRenderer>();
        }

        // Set IsPlayer
        IsPlayer = User?.Components.TryGet<PlayerController>(out var _) ?? false;
    }

    public override void EnableGo(bool enable)
    {
        GameObject.Enabled = enable;
    }
}
