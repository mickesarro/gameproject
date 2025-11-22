using Sandbox;

namespace Shooter;

public class SpawnPoint : Component {
    public Vector3 Position => GameObject.WorldPosition;
    public Rotation Rotation => GameObject.WorldRotation;
}

public sealed class TeamSpawnPoint : SpawnPoint
{
    // At the moment only an int
    [Property] public int Team { get; set; }
}
