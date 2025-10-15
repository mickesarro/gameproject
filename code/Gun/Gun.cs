using Sandbox;

/// <summary>
/// Base class for gun behaviour
/// </summary>
public sealed class Gun : Component, IWeapon, ICollectable
{
	[Property, Sync] public GameObject User { get; set; }
	[Property, RequireComponent] private GunData gunData { get; set; }
	[Property] public string Name { get; set; } = "Gun";

	public WeaponType WeaponType => gunData.WeaponType;

	public GunData GunData => gunData;

	private FireData FireData; // Just for convenience

	private SkinnedModelRenderer modelRenderer;
	private BBox playerBBox;

	protected override void OnAwake()
	{
		if (gunData == null || gunData.PrimaryFireData == null)
		{
			Log.Error( "[Gun] Gun data incomplete!" );
			DestroyGameObject();
			return;
		}

		modelRenderer = gunData?.Viewmodel.Components.Get<SkinnedModelRenderer>( true );

		FireData = gunData.PrimaryFireData;
	}

	protected override void OnStart()
	{
		base.OnStart();

		// If the player picks the weapon, it wont have a User pre-set
		User ??= GameObject?.Parent;

		playerBBox = User != null ? User.GetComponent<BBox>() : default;

		shootInterval = 60f / FireData.RPM;
	}

	private float shootInterval = 0.0f; 
	private float elapsed = 0.0f;
	protected override void OnUpdate()
	{
		if ( IsProxy ) return;

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
		if ( input && elapsed > shootInterval )
		{
			Shoot();
			elapsed = 0.0f;
		}

		elapsed += Time.Delta;
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
			SetAnimation( "fire", true );
		}
		else if ( FireData.BulletType == BulletType.Projectile )
		{
			FireProjectile();
			SetAnimation( "fire", true );
		}
	}

	// Small utility for now
	[Rpc.Broadcast]
	private void SetAnimation(string name, bool state) => modelRenderer?.Parameters.Set( name, state );

	private void FireBullet()
	{
		if (FireData.AmmoLeft == 0)
		{
			// Reload
		}
		--FireData.AmmoLeft;

		// Shoot from the viewport
		var screenCenter = Game.ActiveScene.Camera.WorldPosition; // Might actually be the bottom of camera
		var endPoint = screenCenter + (WorldTransform.Forward * 9999);

		var traceRay = TraceBullet(screenCenter, endPoint, toIgnore: User);

		var traceGo = traceRay.GameObject;
		if ( traceRay.Hit && traceGo.GetComponent<IDamageable>() is IDamageable damageable )
		{
			damageable.OnDamage( new DamageInfo()
				{
					Damage = FireData.Damage,
					Attacker = User,
					Position = traceRay.HitPosition,
				}
			);
		}

		SpawnTracer( traceRay.HitPosition );
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

	private void SpawnTracer( Vector3 target )
	{
		if ( FireData.BulletData.Tracer == null ) return;

		// Shoot visual tracer for bullet
		var clnfg = new CloneConfig { Name = "tracer", StartEnabled = true, Transform = GunData.BarrelEnd.WorldTransform };

		var go = FireData.BulletData.Tracer.Clone( clnfg );

		go.GetComponent<BeamEffect>().TargetPosition = target;

	}

	private void FireProjectile()
	{
		if (FireData.BulletData.ProjectilePrefab == null)
		{
			Log.Error( "No projectile prefab supplied, aborting." );
			return;
		}

		var projectile = FireData.BulletData.ProjectilePrefab
			.Clone( gunData.BarrelEnd.WorldTransform );

		projectile.NetworkSpawn();
	}

	public void Collect( GameObject interactor )
	{
		User = interactor;

		IPlayerEvent.PostToGameObject( interactor, e => e.OnItemAdded( this ) );
	}

	[Rpc.Broadcast]
	public void EnableGo( bool enable ) => GameObject.Enabled = enable;

}
