using System.Numerics;
using Sandbox;

public class BlastEffect : GameObject
{
    [Property] public float Radius { get; set; } = 200.0f; // Tweak the blast radius
    [Property] public float BlastForce { get; set; } = 500.0f; // Tweak blast force
    [Property] public float Damage { get; set; } = 50.0f; // Ehkä joskus joku damage mikä riippuu etäisyydestä?

    public void TriggerBlast( Vector3 position )
    {
        //PlayBlastEffect(position); // Placeholder for visual/audio effects

        // Get nearby objects
        var nearbyObjects = PhysicsUtils.GetGameObjectsInSphere( position, Radius );
    
        foreach ( var obj in nearbyObjects )
        {
            // If PlayerController, apply force
            if ( obj.GetComponent<PlayerController>() is PlayerController player )
            {
                var direction = (obj.WorldPosition - position).Normal;
                //Apply force to the direction
                player.Punch( direction * BlastForce );
            }

        }

        }

    }