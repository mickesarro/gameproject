using Sandbox;

public enum WeaponType { 
	Primary = 0,
	Secondary = 1,
	Melee = 2,
	Total = 3
}

public interface IWeapon
{
	public WeaponType WeaponType { get; }
	public GameObject User { get; set; }
	public void Shoot();
}
