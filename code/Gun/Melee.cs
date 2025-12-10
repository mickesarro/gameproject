using Shooter.Sounds;
using Sandbox;

namespace Shooter;

/// <summary>
/// Simple melee weapon.
/// Inherits from Gun for inventory compatibility, but ignores GunData.
/// </summary>
public sealed class MeleeWeapon : Component, IWeapon, ICollectable
{
    [Property, RequireComponent] private MeleeData meleeData { get; set; }

    // This needs to be solved in some other way
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

    [Property] public string Name { get; set; } = "Melee";

    private TimeSince timeSinceLastAttack;

    /// <summary>
    /// Performs melee attack and returns true if it hit a target.
    /// </summary>
    public void Attack()
    {
        if ( User == null ) return;

        if ( timeSinceLastAttack < (meleeData?.Cooldown) )
            return;

        timeSinceLastAttack = 0;

        // Trace start and direction
        var startPos = IsPlayer ? Game.ActiveScene.Camera.WorldPosition : User.WorldTransform.Position + Vector3.Up * 50f;
        var forward  = IsPlayer ? Game.ActiveScene.Camera.WorldRotation.Forward : User.WorldTransform.Forward;
        var endPos   = startPos + forward * (meleeData?.Range ?? 100f);

        var trace = Game.ActiveScene.Trace
            .Ray( startPos, endPos )
            .UseHitboxes( true )
            .Size( meleeData?.HitRadius ?? 30f )
            .IgnoreGameObjectHierarchy( User )
            .Run();

        if (trace.Hit && trace.GameObject.GetComponent<IDamageable>() is IDamageable target)
        {
            var damageInfo = new DamageInfo
            {
                Damage = meleeData.Damage,
                Attacker = User,
                Position = trace.HitPosition
            };
            if ( !IsPlayer )
            {
                damageInfo.Tags.Add( "npc" );
            }
            // For future reference: This line makes hitmarker show for melee
            IDamageEvent.Post( e => e.OnDamage( trace.GameObject, damageInfo ) );
            target.OnDamage( damageInfo );
        }

        SoundManager.PlayLocal(SoundManager.SoundType.Punch);

        SetAnimation( modelType.ViewModel, "b_attack", true );
    }

    private enum modelType
    {
        ViewModel,
        WorldModel
    }

    // Small utility for now
    [Rpc.Broadcast]
    private void SetAnimation( modelType type, string name, bool state )
    {
        // Could make animation player own component
        // or make a base abstract class for weapons
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
        if ( IsProxy || User == null ) return;

        if ( Input.Pressed( "attack1" ) )
        {
            Attack();
        }
    }


    protected override void OnAwake()
    {
        base.OnAwake();

        User ??= GameObject.Parent.Components.TryGet<ICharacterBase>(out _) ? GameObject.Parent : null;
        if ( IsProxy || User == null ) return;

        if ( meleeData == null || meleeData.ViewModel == null )
        {
            Log.Error( "[Melee] Melee data incomplete!" );
            DestroyGameObject();
            return;
        }

        viewModelRenderer = meleeData.ViewModel.GetComponent<SkinnedModelRenderer>();

        // Set IsPlayer
        IsPlayer = User.Components.TryGet<PlayerController>( out var _ );
    }

    public void EnableGo(bool enable)
    {
        GameObject.Enabled = enable;
    }

    public GameObject GetGameObject() => GameObject;

    public void Collect( GameObject interactor )
    {
        User = interactor;
        IPlayerEvent.PostToGameObject( interactor, e => e.OnItemAdded( this ) );
    }
}
