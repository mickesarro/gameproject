using Sandbox;

namespace Shooter;

/// <summary>
/// Simple melee weapon.
/// Inherits from Gun for inventory compatibility, but ignores GunData.
/// </summary>
public sealed class MeleeWeapon : Component, IWeapon, ICollectable
{
    [Property, RequireComponent] private MeleeData meleeData { get; set; }
    public GunData GunData => null;

    [Property, Sync]
    public GameObject User
    {
        get => _user;
        set
        {
            _user = value;
        }
    }
    private GameObject _user;

    public MeleeData MeleeData => meleeData;

    public bool IsPlayer { get; private set; } // To not run OnUpdate on NPCs

    private SkinnedModelRenderer viewModelRenderer;
    private SkinnedModelRenderer playerModelRenderer;

    public WeaponType WeaponType => WeaponType.Melee;

    [Property] public string Name { get; set; } = "Gun";

    private TimeSince timeSinceLastAttack;

    /// <summary>
    /// Performs melee attack and returns true if it hit a target.
    /// </summary>
    public void Attack()
    {
        if (timeSinceLastAttack < (meleeData?.Cooldown ?? 0.5f))
            return;

        timeSinceLastAttack = 0;

        if (User == null) return;

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

    enum modelType
    {
        ViewModel,
        WorldModel
    }

    // Small utility for now
    [Rpc.Broadcast]
    private void SetAnimation( modelType type, string name, bool state )
    {
        switch ( type )
        {
            case modelType.ViewModel:
                viewModelRenderer?.Parameters.Set( name, state );
                break;

            case modelType.WorldModel:
                playerModelRenderer?.Parameters.Set( name, state );
                break;
        }
    }

    protected override void OnUpdate()
    {
        if (IsProxy || User == null) return;

        if (Input.Pressed("attack1"))
        {
            Attack();
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

    public void EnableGo(bool enable)
    {
        GameObject.Enabled = enable;
    }

    public void Collect( GameObject interactor )
    {
        throw new System.NotImplementedException();
    }
}
