using Sandbox;

/// <summary>
/// Can be used to respond to hitting enemies with e.g. hitmarkers.
/// </summary>
public interface IDamageEvent : ISceneEvent<IDamageEvent>
{
	void OnDamage( GameObject receiver, DamageInfo damageInfo = default ) { }
}
