using System;
using System.Collections.Generic;
using Sandbox;

namespace Shooter.UISystem;

/// <summary>
/// Manages showing and hiding different UI layers in the game.
/// </summary>
public class UIManager : SingletonBase<UIManager>
{
    [Property] private UILayer StartLayer { get; set; }
    [Property] private List<UILayer> UILayers { get; set; } = new();

    private readonly Dictionary<Type, UILayer> UILayerLookup = new();
    private readonly Stack<UILayer> layerHistory = new();
    private UILayer currentLayer;

    protected override void OnAwake()
    {
        base.OnAwake();

        ShowLayer(StartLayer, false);

        foreach (var layer in UILayers)
        {
            UILayerLookup.TryAdd(layer.GetType(), layer);
            if (layer != StartLayer)
                layer.Hide();
        }
    }

    private void SwitchToLayer(UILayer layer, bool addToHistory)
    {
        if (currentLayer != null && !layer.IsOverlay)
        {
            if (addToHistory)
                layerHistory.Push(currentLayer);

            currentLayer.Hide();
        }

        currentLayer = layer;
    }

    public void ShowLayer<T>(bool addToHistory = true) where T : UILayer
    {
        ShowLayer(SearchLayer<T>(), addToHistory);
    }

    public void ShowLayer(UILayer layer, bool addToHistory = true)
    {
        if (layer == null) return;
        SwitchToLayer(layer, addToHistory);
        layer.Show();
    }

    public void ShowLayerWithData<T>(object data, bool addToHistory = true) where T : UILayer
    {
        ShowLayerWithData(SearchLayer<T>(), data, addToHistory);
    }

    public void ShowLayerWithData(UILayer layer, object data, bool addToHistory = true)
    {
        if (layer == null) return;
        SwitchToLayer(layer, addToHistory);
        layer.Show(data);
    }

    public void ToggleLayer<T>(object data = null) where T : UILayer
    {
        var layer = SearchLayer<T>();
        if (layer == null) return;

        bool isVisible = layer.GameObject.Active;
        if (!isVisible)
        {
            if (data != null)
                ShowLayerWithData(layer, data);
            else
                ShowLayer(layer);
        }
        else
        {
            ResetToStartLayer();
        }
    }

    public void ToggleLayer<T>(bool isVisible) where T : UILayer
    {
        var layer = SearchLayer<T>();
        if (layer == null) return;

        if (isVisible)
            ShowLayer(layer);
        else
            ResetToStartLayer();
    }

    public void ShowLastLayer()
    {
        if (layerHistory.Count > 0)
            ShowLayer(layerHistory.Pop(), false);
        else
            ShowLayer(StartLayer, false);
    }

    public void ResetToStartLayer()
    {
        layerHistory.Clear();
        ShowLayer(StartLayer, false);
    }

    public void RemoveLayer<T>() where T : UILayer
    {
        RemoveLayer(SearchLayer<T>());
    }

    public void RemoveLayer(UILayer layer)
    {
        if (layer == null) return;

        UILayers.Remove(layer);
        UILayerLookup.Remove(layer.GetType());

        if (layer == currentLayer)
            currentLayer = null;
    }

    public UILayer SearchLayer<T>() where T : UILayer
    {
        return UILayerLookup.TryGetValue(typeof(T), out var layer) ? layer : null;
    }
}
