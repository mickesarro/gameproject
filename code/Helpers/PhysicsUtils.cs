using Sandbox;

namespace Shooter.Helpers;

/// <summary>
/// Static utility class for physics calculations
/// </summary>
public static class PhysicsUtils
{
	/// <summary>
	/// Returns all the gameobjects in the specified sphere
	/// </summary>
	/// <param name="center">Center of the sphere</param>
	/// <param name="radius">Radius of the sphere</param>
	/// <returns></returns>
    public static IEnumerable<GameObject> GetGameObjectsInSphere(Vector3 center, float radius, bool logEnabled = false)
    {
        // Current scene
        var scene = Game.ActiveScene;
        if ( scene == null )
        {
            // Bail out if no scene
            return Enumerable.Empty<GameObject>();
        }

		// List of objects in a sphere
		var objectsInSphere = new List<GameObject>();

		// Create a Sphere instance
		var sphere = new Sphere(center, radius);

        // Iterate through GameObjects in the sphere
        foreach ( var body in scene.FindInPhysics( sphere ) )
        {
            if ( body is GameObject obj )
            {
                objectsInSphere.Add( obj );
                
                //For logging all the components in the sphere
                if (logEnabled)
				{
					foreach ( var comp in obj.Components.GetAll<Component>() )
					{
						Log.Info( "hit component: " + comp.GetType() );
					}
				}

            }
        }

        return objectsInSphere;
    }
}
