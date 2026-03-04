using System;
using Sandbox;

namespace Shooter.UISystem;

/// <summary>
/// Is responsible for showing and hiding different UI layers.
/// </summary>
public class UIManager : SingletonBase<UIManager>
{

    [Property] private UILayer StartLayer { get; set; }
    
    [Property] private List<UILayer> UILayers { get; set; } = new();
    private readonly Dictionary<Type, UILayer> UILayerLookup = new();

    private readonly Stack<UILayer> layerHistory = new();

    public UILayer CurrentLayer { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();

        ShowLayer( StartLayer, false );

        foreach ( var layer in UILayers )
        {
            UILayerLookup.TryAdd( layer.GetType(), layer );
            if ( layer != StartLayer )
            {
                layer.Hide( data: "UIManager" );
            }
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        StartLayer.Show( data: "UIManager" );
    }

    /// <summary>
    /// Handles all but calling the specific layer Show method.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="addToHistory"></param>
    private void SwitchToLayer( UILayer layer, bool addToHistory )
    {
        if ( layer == null ) return;

        if ( CurrentLayer != null )
        {
            if ( addToHistory )
            {
                layerHistory.Push( CurrentLayer );
            }
            if ( !layer.IsOverlay )
            {
                CurrentLayer.Hide();
            }
        }

        // Might add data option to this and conditionally call
        // layers Show method to unify the methods below.

        CurrentLayer = layer;
    }

    /// <summary>
    /// Display a UI layer of type T.
    /// </summary>
    /// <typeparam name="T">The type of UI layer to be displayed.</typeparam>
    /// <param name="addToHistory">Save in the history stack.</param>
    public void ShowLayer<T>( bool addToHistory = true ) where T : UILayer
    {
        ShowLayer( SearchLayer<T>(), addToHistory );
    }

    /// <summary>
    /// Display a specific UI layer.
    /// </summary>
    /// <param name="layer">The layer to be displayed.</param>
    /// <param name="addToHistory">Save in the history stack.</param>
    public void ShowLayer( UILayer layer, bool addToHistory = true )
    {
        SwitchToLayer( layer, addToHistory );
        layer?.Show();
    }

    /// <summary>
    /// Display a UI layer of type T.
    /// </summary>
    /// <typeparam name="T">The type of UI layer to be displayed.</typeparam>
    /// <param name="addToHistory">Save in the history stack.</param>
    public void ShowLayerWithData<T>( object data, bool addToHistory = true ) where T : UILayer
    {
        ShowLayerWithData( SearchLayer<T>(), data, addToHistory );
    }

    /// <summary>
    /// Display a UI layer of type T.
    /// </summary>
    /// <param name="layer">The layer which to show.</param>
    /// <param name="data">The data to pass.</param>
    /// <param name="addToHistory">Save in the history stack.</param>
    public void ShowLayerWithData( UILayer layer, object data, bool addToHistory = true )
    {
        SwitchToLayer( layer, addToHistory );
        layer?.Show( data );
    }

    /// <summary>
    /// Can be used with toggling behavior like keyboard events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ToggleLayer<T>(object data = null) where T : UILayer
    {
        var layer = SearchLayer<T>();
        ToggleLayer(layer, layer.GameObject.Active, data);
    }

    /// <summary>
    /// Can be used with toggling behavior like keyboard events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ToggleLayer<T>(bool isVisible) where T : UILayer
    {
        ToggleLayer(SearchLayer<T>(), isVisible);
    }

    private void ToggleLayer( UILayer layer, bool isVisible, object data = null )
    {
        if ( !isVisible )
        {
            if ( data != null )
            {
                ShowLayerWithData( layer, data );
            }
            else
            {
                ShowLayer( layer );
            }
        }
        else
        {
            ShowLastLayer();
        }
    }

    /// <summary>
    /// Display the last layer in the stack.
    /// </summary>
    public void ShowLastLayer()
    {
        // Last already in the stack or the first one
        if ( layerHistory.Count > 0 )
        {
            ShowLayer( layerHistory.Pop(), false );
        }
        else
        {
            ResetToStartLayer();
        }
    }

    /// <summary>
    /// Goes back to the starting layer.
    /// Resets the layer history.
    /// </summary>
    public void ResetToStartLayer()
    {

        foreach ( var layer in layerHistory )
        {
            layer?.Hide();
        }
        layerHistory.Clear();

        ShowLayer( StartLayer, false );
    }

    public void RemoveLayer<T>() where T : UILayer
    {
        RemoveLayer( SearchLayer<T>() );
    }

    public void RemoveLayer( UILayer layer )
    {
        if ( layer != null )
        {
            UILayers.Remove( layer );
            UILayerLookup.Remove( layer.GetType() );
            if ( layer == CurrentLayer )
            {
                CurrentLayer = null;
            }
        }
    }

    private UILayer SearchLayer<T>() where T : UILayer
    {
        return UILayerLookup.TryGetValue( typeof( T ), out var layer ) ? layer : null;
    }
}
