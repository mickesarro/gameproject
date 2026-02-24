/*
 * This file is adapted from Facepunch's MIT licensed S&box repository at:
 * https://github.com/Facepunch/sbox-public/blob/master/engine/Sandbox.Engine/Scene/Components/Game/PlayerController/PlayerController.Utility.cs
 * 
 * Copyright (c) 2025 Facepunch Studios Ltd
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Shooter;

public sealed class CharacterRagdoll : Component
{
    [Property] private SkinnedModelRenderer CharacterRenderer { get; set; }

    /// <summary>
    /// Create a ragdoll gameobject version of our render body.
    /// </summary>
    public GameObject CreateRagdoll(Vector3? velocity , string name = "Ragdoll")
    {
        var go = new GameObject( true, name );
        // go.Tags.Add( "ragdoll" );
        go.WorldTransform = WorldTransform;

        var originalBody = CharacterRenderer ?? GetComponentInChildren<SkinnedModelRenderer>();

        if ( !originalBody.IsValid() )
            return go;

        var mainBody = go.Components.Create<SkinnedModelRenderer>();
        mainBody.CopyFrom( originalBody );
        mainBody.UseAnimGraph = false;

        // copy the clothes
        foreach ( var clothing in originalBody.GameObject.Children.SelectMany( x => x.Components.GetAll<SkinnedModelRenderer>() ) )
        {
            if ( !clothing.IsValid() ) continue;

            var newClothing = new GameObject( true, clothing.GameObject.Name )
            {
                Parent = go
            };

            var item = newClothing.Components.Create<SkinnedModelRenderer>();
            item.CopyFrom( clothing );
            item.BoneMergeTarget = mainBody;
        }

        var physics = go.Components.Create<ModelPhysics>();
        physics.Model = mainBody.Model;
        physics.Renderer = mainBody;
        physics.CopyBonesFrom( originalBody, true );
        
        foreach (var body in physics.Bodies)
        {
            if (velocity != null) body.Component.Velocity += velocity.Value;
    
            foreach (var shape in body.Component.PhysicsBody.Shapes)
            {
                if (shape.Collider.IsValid())
                    shape.Collider.ColliderFlags = ColliderFlags.IgnoreTraces;
            }
        }

        var d = go.Components.Create<DestroyTimer>( false );
        d.Delay = 10.0f;
        d.Enabled = true;
        
        return go;
    }
}
