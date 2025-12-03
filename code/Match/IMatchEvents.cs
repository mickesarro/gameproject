namespace Shooter;

/// <summary>
/// Defines events for use during matches.
/// </summary>
public interface IMatchEvents : ISceneEvent<IMatchEvents>
{
	void OnGameEnd() { }
	void OnGameStart() { }

	void OnPlayerJoined() { }
	void OnPlayerLeft() { }

	void OnKill( PlayerController killed, DamageInfo damageInfo ) { }
}
