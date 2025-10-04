using Sandbox;

/// <summary>
/// Base class for gun behaviour
/// </summary>
public sealed class Gun : Component
{
	[Property] public PlayerController User { get; set; }
	[Property] private GunData gunData { get; set; }
	private BulletData bulletData; // Just for convenience

	private SkinnedModelRenderer modelRenderer;
	private BBox playerBBox;

	protected override void OnAwake()
	{
		if (gunData == null || gunData.BulletData == null)
		{
			Log.Error( "[Gun] Gun data incomplete!" );
			DestroyGameObject();
		}

		modelRenderer = gunData?.Viewmodel.Components.Get<SkinnedModelRenderer>();

		bulletData = gunData.BulletData;
	}

	protected override void OnStart()
	{
		base.OnStart();

		// If the player picks the weapon, it wont have a User pre-set
		User ??= GameObject?.Parent.GetComponent<PlayerController>();

		Log.Info( User );
		Log.Info( GameObject?.Parent );

		if (User != null)
		{
			playerBBox = User.GetComponent<BBox>();
			Log.Info( "Bounding box found" );
		}
	}

	private float elapsed = 0.0f;
	protected override void OnUpdate()
	{
		// Should be implemented with some form of events perhaps
		if ( Input.Pressed( "attack1" ) && elapsed > gunData.LoadTime )
		{
			Shoot();
			elapsed = 0.0f;

		}

		elapsed += Time.Delta;
	}

	private void Shoot()
	{
		if ( gunData.ShootType == ShootType.Bullet )
		{
			FireBullet();
		}
		else if ( gunData.ShootType == ShootType.Projectile )
		{
			FireProjectile();
		}
	}

	
	private void FireBullet()
	{
		// Shoot from the viewport
		var screenCenter = Game.ActiveScene.Camera.WorldPosition; // Might actually be the bottom of camera
		var endPoint = screenCenter + (WorldTransform.Forward * int.MaxValue);

		TraceBullet(screenCenter, endPoint, toIgnore: User.GameObject);
	}

	private void TraceBullet(Vector3 start, Vector3 end, float radius = 10.0f, GameObject toIgnore = null)
	{
		var traceRay = Game.ActiveScene.Trace
			.Ray( start, end )
			.Size( radius )
			.UseHitboxes( true )
			.IgnoreGameObjectHierarchy( toIgnore )
			.Run();

		var traceGo = traceRay.GameObject;

		if ( traceRay.Hit && traceGo.Tags.Has( "player" ) ) 
		{
			traceGo.GetComponent<IDamageable>()
				?.OnDamage( new DamageInfo()
				{
					Damage = bulletData.Damage,
					Attacker = User.GameObject,
					Position = traceRay.HitPosition,
				}
			);

			SpawnTracer();

			Log.Info( "Ray hit" ); // Remove later
		}
	}

	private void SpawnTracer()
	{
		// Shoot visual tracer for bullet
	}

	private void FireProjectile()
	{
		if (bulletData.ProjectilePrefab == null)
		{
			Log.Error( "No projectile prefab supplied, aborting." );
		}

		bulletData.ProjectilePrefab
			.Clone(
				gunData.BarrelEnd.WorldTransform
			);
	}

}
