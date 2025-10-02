using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Sandbox;
using Sandbox.ModelEditor.Nodes;

public static class PhysicsUtils
{
    public static IEnumerable<GameObject> GetGameObjectsInSphere(Vector3 center, float radius)
    {
        // List of objects in a sphere
        var objectsInSphere = new List<GameObject>();

        // Current scene
        var scene = Game.ActiveScene;
        if ( scene == null )
        {
            // Bail out if no scene
            return objectsInSphere;
        }

        // Create a Sphere instance
        var sphere = new Sphere(center, radius);

        // Iterate through GameObjects in the sphere
        foreach ( var body in scene.FindInPhysics( sphere ) )
        {
            if ( body is GameObject obj )
            {
                objectsInSphere.Add( obj );
                
                //For logging all the components in the sphere
                foreach ( var comp in obj.Components.GetAll<Component>() )
                {
                    Log.Info( "hit component: " + comp.GetType() );
                }

            }
        }

        return objectsInSphere;
    }
}