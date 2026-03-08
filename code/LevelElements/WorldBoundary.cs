namespace Shooter;

public sealed class WorldBoundary : Component, Component.ITriggerListener 
{
    public void OnTriggerEnter( Collider other )
    {
        if ( other.Tags.Has( "shootable" ) )
        {
            other.GameObject.Root.WorldPosition = Spawner.GetSpawnPoint().WorldPosition;
        }
    }
}
