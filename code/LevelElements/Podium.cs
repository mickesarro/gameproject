using Shooter.UI;

namespace Shooter;

public sealed class Podium : Component
{
    [Property] private SpawnPoint First { get; set; }
    [Property] private SpawnPoint Second { get; set; }
    [Property] private SpawnPoint Third { get; set; }

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

            InstantiateCharacter( player.GameObject, spawnPoints[position] );

            if ( position == 2 ) break;
            position++;
        }

    }

    private void InstantiateCharacter( GameObject character, SpawnPoint spawnPoint )
    {
        var body = character.GetComponentInChildren<SkinnedModelRenderer>( includeDisabled: true );
        
        // May not work on the client with NPCs, need to investigate
        var cloned = body.GameObject.Clone(
            new CloneConfig { Parent = GameObject, StartEnabled = true }
        );

        cloned.WorldTransform = spawnPoint.WorldTransform;

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

}
