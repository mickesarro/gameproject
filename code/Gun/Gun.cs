using Sandbox;
using Sandbox.Citizen;

/// <summary>
/// Base class for gun behaviour
/// </summary>
public sealed class Gun : Component, IWeapon, ICollectable
{
	[Property, Sync] public GameObject User { get; set; }
	private bool isPlayer; // To not run OnUpdate on NPCs
	[Property, RequireComponent] private GunData gunData { get; set; }
	[Property] public string Name { get; set; } = "Gun";

	public WeaponType WeaponType => gunData.WeaponType;

	public GunData GunData => gunData;

	private FireData FireData; // Just for convenience

	private SkinnedModelRenderer viewModelRenderer;
	private SkinnedModelRenderer playerModelRenderer;
	private BBox playerBBox;
	private AmmoInventory AmmoInventory;

	protected override void OnAwake()
	{
		playerModelRenderer?.Parameters.Set( "holdtype", gunData.holdType.AsInt() );
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
	}

	protected override void OnStart()
	{
		if ( IsProxy ) return;
		base.OnStart();

		// If the player picks the weapon, it wont have a User pre-set
		User ??= GameObject?.Parent;
		
		if ( User == null )
		{
			Log.Info( "No user found." );
			return;
		}

		isPlayer = User.Components.TryGet<PlayerController>( out _ );

		AmmoInventory = User.GetComponent<AmmoInventory>();

		playerBBox = User?.GetComponent<BBox>() ?? default;
		var playerBody = User?.Children.Find(obj => obj.Name == "Body" );
		playerModelRenderer = playerBody?.GetComponent<SkinnedModelRenderer>(true);
		playerModelRenderer?.Parameters.Set( "holdtype", gunData.holdType.AsInt() );
		Log.Info( playerModelRenderer );

		shootInterval = 60f / FireData.RPM;
		timeSinceLastShot = 0; // Seems the sandbox time.now is messed up at object creation
	}

	private float shootInterval = 0.0f; 
	private TimeSince timeSinceLastShot = 0;
	protected override void OnUpdate()
	{
		if ( IsProxy || !isPlayer ) return;

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
		if (FireData.AmmoLeft == 0)
		{
			Reload();
			return;
		}
		--FireData.AmmoLeft;

		// Shoot from the viewport
		var screenCenter = Game.ActiveScene.Camera.WorldPosition; // Might actually be the bottom of camera
		var endPoint = screenCenter + (WorldTransform.Forward * 9999);

		var traceRay = TraceBullet(screenCenter, endPoint, toIgnore: User);

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
			.RemoveAmmo( FireData.AmmoType, FireData.MaxAmmo );

		FireData.AmmoLeft = reloaded;

		// Should do some animation etc. as well
		timeSinceLastShot -= FireData.LoadTime; // Better solution required
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
		if ( FireData.AmmoLeft == 0 )
		{
			Reload();
			return;
		}
		--FireData.AmmoLeft;

		var projectile = FireData.BulletData.ProjectilePrefab
			.Clone( gunData.BarrelEnd.WorldTransform );

		// A better solution is required for final product
		projectile.GetComponent<Projectile>().Attacker = User;

		projectile.NetworkSpawn();

		if ( FireData.AmmoType == AmmoType.Rocket ) // If bazooka, reload
		{
			Reload();
		}
	}

	public void Collect( GameObject interactor )
	{
		User = interactor;

		IPlayerEvent.PostToGameObject( interactor, e => e.OnItemAdded( this ) );
	}

	[Rpc.Broadcast]
	public void EnableGo( bool enable ) => GameObject.Enabled = enable;
}
