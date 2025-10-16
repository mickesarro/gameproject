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
	private AmmoInventory AmmoInventory;

	protected override void OnAwake()
	{
		if ( IsProxy ) return;
		if (gunData == null || gunData.PrimaryFireData == null)
		{
			Log.Error( "[Gun] Gun data incomplete!" );
			DestroyGameObject();
			return;
		}

		modelRenderer = gunData?.Viewmodel.Components.Get<SkinnedModelRenderer>( true );

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

		AmmoInventory = User.GetComponent<AmmoInventory>();

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
		}
		else if ( FireData.BulletType == BulletType.Projectile )
		{
			FireProjectile();
		}
	}

	// Small utility for now
	[Rpc.Broadcast]
	private void SetAnimation(string name, bool state) => modelRenderer?.Parameters.Set( name, state );

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
			damageable.OnDamage( new DamageInfo()
				{
					Damage = FireData.Damage,
					Attacker = User,
					Position = traceRay.HitPosition,
				}
			);

			SpawnTracer();

			Log.Info( "Ray hit" ); // Remove later
		}
		SetAnimation( "fire", true );
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
		elapsed -= FireData.LoadTime; // Better solution required
	}

	private void SpawnTracer()
	{
		// Shoot visual tracer for bullet
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

		SetAnimation( "fire", true );
	}

	public void Collect( GameObject interactor )
	{
		User = interactor;

		IPlayerEvent.PostToGameObject( interactor, e => e.OnItemAdded( this ) );
	}

	[Rpc.Broadcast]
	public void EnableGo( bool enable ) => GameObject.Enabled = enable;

}
