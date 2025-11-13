using Sandbox;
using Shooter.Sounds;

namespace Shooter;

/// <summary>
/// Simple melee weapon implementation.
/// </summary>
public sealed class MeleeWeapon : Component, IWeapon, ICollectable, IPlayerEvent
{
  
    [Property, Sync] public GameObject User { get; set; }

    [Property, RequireComponent] private MeleeData meleeDatai { get; set; }

    [Property] public string Name { get; set; } = "Melee";

   
    public WeaponType WeaponType => WeaponType.Melee;

    public MeleeData MeleeData => meleeDatai;
    public GunData GunData => null;

    private TimeSince timeSinceLastAttack;

    /// <summary>
    ///  melee attack
    /// </summary>
    public void Attack()
    {
        Log.Info("toimii");
        if (timeSinceLastAttack < (meleeDatai?.Cooldown ?? 0.5f)) return;
        timeSinceLastAttack = 0;

        if (User == null) return;

        var startPos = User.WorldTransform.Position;
        var forward = User.WorldTransform.Forward;

        var endPos = startPos + forward * (meleeDatai?.Range ?? 100f);

        var trace = Game.ActiveScene.Trace
            .Ray(startPos, endPos)
            .UseHitboxes(true)
            .Size(meleeDatai?.HitRadius ?? 10f)
            .IgnoreGameObjectHierarchy(User)
            .Run();

        if (trace.Hit && trace.GameObject.GetComponent<IDamageable>() is IDamageable target)
        {
            var damageInfo = new DamageInfo
            {
                Damage = meleeDatai.Damage,
                Attacker = User,
                Position = trace.HitPosition
            };

            target.OnDamage(damageInfo);
        }
    }

    public void Shoot() => Attack();

    public void Collect(GameObject interactor)
    {
        User = interactor;
        IPlayerEvent.PostToGameObject(interactor, e => e.OnItemAdded(this));
    }

    public void EnableGo(bool enable)
    {
        GameObject.Enabled = enable;
    }

    protected override void OnUpdate()
    {
        if (IsProxy) return;
        if (User == null) return;

        if (Input.Pressed("attack1"))
        {
            Attack();
        }
    }

    void IPlayerEvent.OnItemAdded(ICollectable item) { }
    void IPlayerEvent.OnWeaponAdded(IWeapon item) { }
    void IPlayerEvent.OnSwitchItem(ICollectable collectable) { }
    void IPlayerEvent.OnSwitchItem(InventorySlot slot) { }
}
