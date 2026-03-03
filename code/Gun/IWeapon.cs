using Sandbox;

namespace Shooter;

public enum WeaponType { 
    Primary = 0,
    Secondary = 1,
    Railgun = 2,
    Melee = 3,
    Total = 4
}

public enum WeaponRarity
{
    Common,
    Rare,
}

public interface IWeapon
{
	public GunData GunData { get; }
    public MeleeData MeleeData { get; }
	public WeaponType WeaponType { get; }
	public GameObject User { get; set; }
	public void Attack();
    public float CooldownFraction { get; }
    public Texture Icon { get; }
}
