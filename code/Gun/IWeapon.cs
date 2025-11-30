using Sandbox;

namespace Shooter;

public enum WeaponType { 
    Primary = 0,
    Secondary = 1,
    Melee = 2,
    Total = 3
}

public interface IWeapon
{
	public GunData GunData { get; }
    public MeleeData MeleeData { get; }
	public WeaponType WeaponType { get; }
	public GameObject User { get; set; }
	public void Attack();
}
