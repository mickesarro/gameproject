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

	private FireData FireData; // Just for convenience

	private SkinnedModelRenderer viewModelRenderer;
	private SkinnedModelRenderer playerModelRenderer;
	private BBox playerBBox;
	private AmmoInventory AmmoInventory;
	
    private CameraComponent camera;
    
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

        // Could be implemented with tags for example
        IsPlayer = User.Components.TryGet<PlayerController>( out _ );
    }

	protected override void OnStart()
	{
		base.OnStart();
		if ( IsProxy ) return;

        camera = Scene.Camera;
		
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
        
		// This is not ideal and must be made independent later.
		GameObject.WorldPosition = camera.WorldPosition;
		GameObject.WorldRotation = camera.WorldRotation;

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
			Shoot();
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

	public void Shoot()
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

        SoundManager.PlayGlobal( FireData.FiringSound, GameObject.WorldPosition, 1000f, 0.3f );
        timeSinceLastShot = 0.0f;
        
	}

	enum modelType
	{
		ViewModel,
		WorldModel
	}

	// Small utility for now
	[Rpc.Broadcast]
	private void SetAnimation(modelType type, string name, bool state)
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

	private void FireBullet()
	{
        TryReload();

        --FireData.AmmoLeft;

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

			IDamageEvent.Post( e => e.OnDamage( traceGo, damageInfo ) );
			damageable.OnDamage( damageInfo );
		}

		SetAnimation(modelType.ViewModel, "fire", true );
		SetAnimation(modelType.WorldModel, "b_attack", true );
        SetAnimation(modelType.ViewModel, "fire", false );

		SpawnTracer( traceRay.Hit ? traceRay.HitPosition : endPoint );
	}

	private SceneTraceResult TraceBullet(Vector3 start, Vector3 end, float radius = 10.0f, GameObject toIgnore = null)
	{
		var traceRay = Game.ActiveScene.Trace
			.Ray( start, end )
			.Size( radius )
			.UseHitboxes( true )
			.IgnoreGameObjectHierarchy( toIgnore )
			.Run();

		return traceRay;
	}

	private void Reload()
	{
		int reloaded = AmmoInventory
			.RemoveAmmo( FireData.AmmoType, FireData.MaxAmmo ); // Intentionally MaxAmmo as it is the magazine size

		FireData.AmmoLeft = reloaded;

		// Should do some animation etc. as well
		timeSinceLastShot -= FireData.LoadTime; // Better solution required

		SoundManager.PlayGlobal( SoundManager.SoundType.Reload, GameObject.WorldPosition, 500f, 0.5f );
	}

    /// <summary>
    /// Reloads if no ammo left
    /// </summary>
    /// <returns></returns>
    private bool TryReload()
    {
        if ( FireData.AmmoLeft > 0 ) return false;

        SoundManager.PlayGlobal( SoundManager.SoundType.OutOfAmmo, GameObject.WorldPosition, 500f, 0.5f );
        Reload();

        return true;
    }

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

	private void FireProjectile()
	{
        TryReload();

        --FireData.AmmoLeft;
		
		var projectile = FireData.BulletData.ProjectilePrefab
			.Clone( gunData.BarrelEnd.WorldTransform );

		// A better solution is required for final product
		projectile.GetComponent<Projectile>().Attacker = User;

		projectile.NetworkSpawn();
		
		SetAnimation(modelType.ViewModel, "fire", true );
		SetAnimation(modelType.WorldModel, "b_attack", true );
        SetAnimation(modelType.ViewModel, "fire", false );
	}

	public void Collect( GameObject interactor )
	{
		User = interactor;
		IPlayerEvent.PostToGameObject( interactor, e => e.OnItemAdded( this ) );
	}

	[Rpc.Broadcast]
	public void EnableGo( bool enable )
	{
		GameObject.Enabled = enable;
        if ( IsProxy )
        {
            playerModelRenderer?.Parameters?.Set( "holdtype", gunData.holdType.AsInt() );
        }
        else
        {
            gunData.Viewmodel.Enabled = enable;
            HandleProxyAnimations();
        }
	}
}
