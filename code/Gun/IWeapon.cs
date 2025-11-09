using Sandbox;

public enum WeaponType { 
	Melee = 0,
	Smg = 1,
	Rockets = 2,
	RailGun = 3,
	Other = 4,
	Empty = 5,
	Total = 6
}

public interface IWeapon
{
	public GunData GunData { get; }
	public WeaponType WeaponType { get; }
	public GameObject User { get; set; }
	public void Shoot();
}
