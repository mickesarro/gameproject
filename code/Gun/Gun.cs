using Sandbox;
using Shooter.Sounds;

namespace Shooter;

/// <summary>
/// Base class for gun behaviour
/// </summary>
public sealed class Gun : Component, IWeapon, ICollectable
{
	[Property, Sync]
	public GameObject User
	{
		get => _user;
		set
		{
			_user = value;
			HandleProxyAnimations();
		}
	}
	private GameObject _user;
	public bool IsPlayer { get; private set; } // To not run OnUpdate on NPCs

	[Property, RequireComponent] private GunData gunData { get; set; }
	[Property] public string Name { get; set; } = "Gun";

	public WeaponType WeaponType => gunData.WeaponType;

	public GunData GunData => gunData;

    public MeleeData MeleeData => null;

    private FireData FireData; // Just for convenience

	private SkinnedModelRenderer viewModelRenderer;
	private SkinnedModelRenderer playerModelRenderer;
	private BBox playerBBox;
	private AmmoInventory AmmoInventory;
	
	private void HandleProxyAnimations()
	{
		if (User == null || gunData == null) return;
		var playerBody = User.Children.Find(obj => obj.Name == "Body");
		playerBody?.Components.TryGet<SkinnedModelRenderer>(out playerModelRenderer);
		playerModelRenderer?.Parameters.Set("holdtype", gunData.holdType.AsInt());
	}

	protected override void OnAwake()
	{
        base.OnAwake();

		if ( IsProxy ) return;
		if (gunData == null || gunData.PrimaryFireData == null)
		{
			Log.Error( "[Gun] Gun data incomplete!" );
			DestroyGameObject();
			return;
		}

		viewModelRenderer = gunData?.Viewmodel.Components.Get<SkinnedModelRenderer>( true );
		
		FireData = gunData.PrimaryFireData;
		if ( FireData.BulletData.ProjectilePrefab == null )
		{
			Log.Error( "No projectile prefab supplied, aborting." );
			DestroyGameObject();
			return;
		}

        IsPlayer = User.Components.TryGet<PlayerController>( out _ );

        // See renderers for comment
        renderers = [.. gunData.Viewmodel.GetComponentsInChildren<SkinnedModelRenderer>( includeDisabled: true, includeSelf: true )];
    }

	protected override void OnStart()
	{
		base.OnStart();
		if ( IsProxy ) return;

		User ??= GameObject.Parent;

		if ( User == null )
		{
			Log.Info( "No user found." );
			return;
		}

		AmmoInventory = User.GetComponent<AmmoInventory>();

		playerBBox = User?.GetComponent<BBox>() ?? default;

		shootInterval = 60f / FireData.RPM;
		timeSinceLastShot = 0; // Seems the sandbox time.now is messed up at object creation
	}

	private float shootInterval = 0.0f; 
	private TimeSince timeSinceLastShot = 0;
	protected override void OnUpdate()
	{
		if ( IsProxy || !IsPlayer ) return;

        if ( Input.Down( "reload" ) ) {
            TryReload( loadAdditive: true );
            return;
        }

		bool input = false;
		switch (FireData.FireType)
		{
			case FireType.FullAuto:
				input = Input.Down( "attack1" );
				break;
			case FireType.SemiAuto:
				input = Input.Pressed( "attack1" );
				break;
		}

		// Should be implemented with some form of events perhaps
		if ( input && CanShoot() )
		{
			Attack();
		}
	}

	/// <summary>
	/// Determines whether gun can be fired or not yet
	/// </summary>
	/// <returns></returns>
	public bool CanShoot()
	{
		// Should likely be added more conditionals
		return timeSinceLastShot > shootInterval;
	}

	public void Attack()
	{
		// For future reference:
		// Both could be components that implement e.g. a IFireable interface.
		// The logic of of the fireable (bullet or projectile) would be handled
		// by their respective fire methods.
		if ( FireData.BulletType == BulletType.Bullet )
		{
			FireBullet();
		}
		else if ( FireData.BulletType == BulletType.Projectile )
		{
			FireProjectile();
		}

        if ( GunData.AutomaticReload && TryReload() ) return;

        timeSinceLastShot = 0.0f;
        
	}

	enum modelType
	{
		ViewModel,
		WorldModel
	}

    // !! This is not optimal and needs to be solved in some other way
    List<SkinnedModelRenderer> renderers = new();

    // Small utility for now
    [Rpc.Broadcast]
	private void SetAnimation(modelType type, string name, bool state)
	{

		switch ( type )
		{
			case modelType.ViewModel:
                if ( IsProxy ) return;
				foreach ( var renderer in renderers )
				{
					renderer?.Set( name, state );
				}
				break;

			case modelType.WorldModel:
				playerModelRenderer?.Set( name, state );
				break;
		}
	}

	private void FireBullet()
	{

		//Check ammo before firing
		if (FireData.AmmoLeft <= 0) {
			TryReload();
			return;

		}
        
        --FireData.AmmoLeft;

        BroadcastSound( FireData.FiringSound, GameObject.WorldPosition, 2000f, 0.3f );

		// Shoot from the viewport
		// var screenCenter = Game.ActiveScene.Camera.WorldPosition; // Might actually be the bottom of camera

		// !! We need a proper solution to the "spawn" point of the bullet
		var startPoint = IsPlayer ? Game.ActiveScene.Camera.WorldPosition : WorldPosition + Vector3.Up;
		var endPoint = startPoint + (WorldTransform.Forward * 9999);

		var traceRay = TraceBullet( startPoint, endPoint, toIgnore: User);

		var traceGo = traceRay.GameObject;
		if ( traceRay.Hit && traceGo.GetComponent<IDamageable>() is IDamageable damageable )
		{
			var damageInfo = new DamageInfo()
			{
				Damage = FireData.Damage,
				Attacker = User,
				Position = traceRay.HitPosition,
			};

            if ( !IsPlayer )
            {
                damageInfo.Tags.Add( "npc" );
            }

			IDamageEvent.Post( e => e.OnDamage( traceGo, damageInfo ) );
			damageable.OnDamage( damageInfo );
		}

        SetAnimation( modelType.ViewModel, "fire", true );
        SetAnimation( modelType.WorldModel, "b_attack", true );

		SpawnTracer( traceRay.Hit ? traceRay.HitPosition : endPoint );
        SpawnBulletHitEffects( traceRay );

    }

	private SceneTraceResult TraceBullet(Vector3 start, Vector3 end, float radius = 10.0f, GameObject toIgnore = null)
	{
        return Game.ActiveScene.Trace
            .Ray( start, end )
            .Size( radius )
            .UseHitboxes( true )
            .IgnoreGameObjectHierarchy( toIgnore )
            .WithoutTags( "movement" )
            .Run();
	}

    private void SpawnBulletHitEffects( SceneTraceResult tr )
    {
        if ( !tr.Hit ) return;

        var bulletImpact = 
            tr.Surface.PrefabCollection.BulletImpact
            ?? tr.Surface.GetBaseSurface()?.PrefabCollection.BulletImpact;

        // Clone the impact prefab at the surface and set the appropriate rotations
        bulletImpact?.Clone
            (
                tr.EndPosition + tr.Normal,
                Rotation.LookAt( tr.Normal )
            ).SetParent( tr.GameObject, keepWorldPosition: true );

        if ( tr.Surface.SoundCollection.Bullet != null )
        {
            BroadcastSound( tr.Surface.SoundCollection.Bullet, tr.EndPosition );
        }
    }

    private void Reload()
	{
		int reloaded = AmmoInventory
			.RemoveAmmo( FireData.AmmoType, FireData.MaxAmmo - FireData.AmmoLeft); // Intentionally MaxAmmo as it is the magazine size

		FireData.AmmoLeft += reloaded;

		// Should do some animation etc. as well
		timeSinceLastShot -= FireData.LoadTime; // Better solution required

		if ( reloaded > 0 ) {
            BroadcastSound( SoundManager.SoundType.Reload, GameObject.WorldPosition, 500f, 0.5f );
		}
		
	}

    /// <summary>
    /// Reloads if no ammo left
    /// </summary>
    /// <returns></returns>
    private bool TryReload( bool loadAdditive = false )
    {
        // Using the same one as shooting for now
        if ( !CanShoot() ) return false;

        if ( !loadAdditive && FireData.AmmoLeft > 0 ) return false;

        if ( FireData.AmmoLeft == 0 )
        {
            SoundManager.PlayGlobal( SoundManager.SoundType.OutOfAmmo, GameObject.WorldPosition, 500f, 0.5f );
        }
        
        Reload();

        return true;
    }

    // [Rpc.Broadcast]
	private void SpawnTracer( Vector3 target )
	{
		// For future reference:
		// If needed, consider object pooling the tracers.

		if ( FireData.BulletData.Tracer == null ) return;

		// Shoot visual tracer for bullet
		var clnfg = new CloneConfig { Name = "tracer", StartEnabled = true, Transform = GunData.BarrelEnd.WorldTransform };

		FireData.BulletData.Tracer.Clone( clnfg )
			.GetComponent<BeamEffect>().TargetPosition = target;
	}

    [Rpc.Broadcast]
    private void BroadcastSound( SoundManager.SoundType soundType, Vector3 position, float range, float volume )
    {
        SoundManager.PlayGlobal( soundType, position, range, volume );
    }

    [Rpc.Broadcast]
    private void BroadcastSound( SoundEvent sound, Vector3 position )
    {
        SoundManager.PlayGlobal( sound, position, sound.Distance, sound.Volume.FixedValue );
    }

    private void FireProjectile()
	{
        //Check ammo before firing
        if ( FireData.AmmoLeft <= 0 )
        {
            TryReload();
            return;
        }

        --FireData.AmmoLeft;

		BroadcastSound( FireData.FiringSound, GameObject.WorldPosition, 6900f, 0.3f );
		
		var projectile = FireData.BulletData.ProjectilePrefab
			.Clone( gunData.BarrelEnd.WorldTransform );

		// A better solution is required for final product
		projectile.GetComponent<Projectile>().Attacker = User;

		projectile.NetworkSpawn();

		SetAnimation(modelType.ViewModel, "fire", true );
		SetAnimation(modelType.WorldModel, "b_attack", true );
	}

	public void Collect( GameObject interactor )
	{
		User = interactor;
		IPlayerEvent.Post( e => e.OnItemAdded( this ) );
	}

	[Rpc.Broadcast]
	public void EnableGo( bool enable )
	{
		GameObject.Enabled = enable;
        if ( IsProxy )
        {
            HandleProxyAnimations();
        }
        else
        {
            playerModelRenderer?.Parameters?.Set( "holdtype", gunData.holdType.AsInt() );
            SetAnimation(modelType.ViewModel, "fire", false );
        }
	}

    public GameObject GetGameObject() => GameObject;
}
