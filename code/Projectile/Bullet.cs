using Sandbox;

public sealed class Bullet : Component
{
	/* The projectile class is not suitable for simple bullets.
	 * Projectiles, such as rpg rockets and grenades, can be modeled individually over time.
	 * In addition, bullets can't really use collider checks but should rather use something like hitscan.
	 * 
	 */
}
