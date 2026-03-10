using System;
using Shooter.UI;

namespace Shooter;

public sealed class Podium : Component
{
    [Property] private SpawnPoint First { get; set; }
    [Property] private SpawnPoint Second { get; set; }
    [Property] private SpawnPoint Third { get; set; }

    private List<SkinnedModelRenderer> renderers = [];
    private readonly List<String> idleanims = ["AvatarMenu_Idle_01", "AvatarMenu_Idle_02"];

    protected override void OnEnabled()
    {
        base.OnEnabled();

        GetComponentInChildren<CameraComponent>( includeDisabled: true )
           .IsMainCamera = true;

        var players = MatchStatsManager.Instance.Tracked
            .OrderByDescending( chr => chr?.Score );

        SpawnPoint[] spawnPoints = [ First, Second, Third ];

        // Could cause weird podium bugs where someone who isn't in top 3
        // is being displayed. But, it is better than having empty podium.
        // This basically fixes the situation where only one or two have kills
        // and for some reason the ones with zero have no bodies.
        int position = 0;
        foreach ( var player in players )
        {
            if ( player?.GameObject == null ) continue;

            InstantiateCharacter( player.GameObject, spawnPoints[position], position );

            if ( position == 2 ) break;
            position++;
        }

    }

    private void InstantiateCharacter( GameObject character, SpawnPoint spawnPoint, int position )
    {
        var body = character.GetComponentsInChildren<SkinnedModelRenderer>( includeDisabled: true )
            .FirstOrDefault( smr => smr.GameObject.Name == "Body" );
        
        // May not work on the client with NPCs, need to investigate
        var cloned = body.GameObject.Clone(
            new CloneConfig { Parent = GameObject, StartEnabled = true }
        );

        cloned.WorldTransform = spawnPoint.WorldTransform;

        foreach ( var renderer in cloned.GetComponents<SkinnedModelRenderer>() )
        {
            renderers.Add( renderer );
            renderer.UseAnimGraph = false;
            renderer.Sequence.Blending = true;
            switch ( position )
            {
                case 0:
                    renderer.Sequence.Name = "AvatarMenu_Entry_Flex";
                    break;
                case 1:
                    renderer.Sequence.Name = "AvatarMenu_Entry_Wave_02";
                    break;
                case 2:
                    renderer.Sequence.Name = "AvatarMenu_ExamineArms_03";
                    break;
            }
            renderer.Sequence.Looping = false;
        }

        var nametag = GameObject.Clone(
            "prefabs/nametag.prefab",
            new CloneConfig { Parent = cloned, StartEnabled = true }
        );

        // Just over the head
        nametag.LocalPosition += new Vector3( 0, 0, 75 );

        // Ironically makes it face the podium camera
        nametag.GetComponent<WorldPanel>().LookAtCamera = false;

        if ( !Network.IsProxy && cloned.Tags.Has( "player" ) )
        {
            nametag.GetComponent<NameTag>() // This is to use the same nametag component and display it still
                .OverrideValues( Name: character.Network.Owner?.DisplayName, isPlayer: false );

            // Loop over added clothing items to make them visible for the owner
            foreach ( var c in cloned.GetComponentsInChildren<SkinnedModelRenderer>() )
            {
                c.RenderType = ModelRenderer.ShadowRenderType.On;
            }
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        // This seems to still throw errors despite previous checks 
        // Therefore this is now riddled with them to reduce our erroring rate
        // The method only runs on idle point of game and on clients

        if ( renderers == null || renderers.Count == 0 ) return;

        foreach ( var renderer in renderers )
        {
            if ( !renderer.IsValid() || renderer?.Sequence == null ) return;

            if ( renderer.Sequence?.IsFinished == true && idleanims != null )
            {
                renderer.Sequence?.Looping = true;
                renderer.Sequence?.Name = idleanims[new Random().Next( 0, idleanims.Count )];
            }
        }
    }
}
