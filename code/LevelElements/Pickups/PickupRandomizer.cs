using System;

namespace Shooter;

/// <summary>
/// Randomizes the pickups in the scene
/// </summary>
public sealed class PickupRandomizer : Component
{
    [Property] private List<PrefabFile> GunPrefabs { get; set; } = new();

    private List<Pickup> GunPickups = new();
    private List<Pickup> HealthPickups = new();

    protected override void OnAwake()
    {
        base.OnAwake();

        if ( !Networking.IsHost ) return;

        foreach ( var item in Game.ActiveScene.GetAllComponents<Pickup>() )
        {
            if ( item is ItemPickup )
            { 
                GunPickups.Add( item );
                item.Collected += FindNewPosition;
            }
            else if ( item is HealthPickup ) HealthPickups.Add( item );

            
        }
    }

    private void FindNewPosition( Pickup pickup )
    {
        var item = pickup.ItemPrefab;
        pickup.ItemPrefab = null;
        pickup.ModelRenderer.Enabled = false;

        var rarity = item.GetMetadata( "Rarity" );
        if ( !Enum.TryParse( rarity, ignoreCase: true, out Pickup.Rarity parsed ) ) return;

        var newPickup = GunPickups.Shuffle()
            .FirstOrDefault( p => p.PickupRarity == parsed );

        newPickup.ItemPrefab = item;
        newPickup.ModelRenderer.Enabled = true;
        newPickup.ModelRenderer.Model = Model.Load( item.GetMetadata( "Model" ) );
    }
	
}
