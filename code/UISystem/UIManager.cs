using Sandbox;

namespace UISystem
{
    /// <summary>
    /// Is responsible for showing and hiding different UI layers.
    /// </summary>
    public class UIManager : SingletonBase<UIManager>
    {

        [Property] private UILayer StartLayer { get; set; }
        [Property] private List<UILayer> UILayers { get; set; } = new();

        private readonly Stack<UILayer> layerHistory = new();

        private UILayer currentLayer;

        protected override void OnAwake()
        {
            base.OnAwake();

            ShowLayer( StartLayer, false );

            foreach ( var layer in UILayers )
            {
                if ( layer != StartLayer )
                {
                    layer.Hide();
                }
            }
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
            if ( currentLayer != null )
            {
                if ( addToHistory )
                {
                    layerHistory.Push( currentLayer );
                }
                currentLayer.Hide();
            }

            layer?.Show();

            currentLayer = layer;
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
        /// <typeparam name="T">The type of UI layer to be displayed.</typeparam>
        /// <param name="addToHistory">Save in the history stack.</param>
        public void ShowLayerWithData( UILayer layer, object data, bool addToHistory = true )
        {
            if ( currentLayer != null )
            {
                if ( addToHistory )
                {
                    layerHistory.Push( currentLayer );
                }
                currentLayer.Hide();
            }

            layer?.Show( data );

            currentLayer = layer;
        }

        /// <summary>
        /// Can be used with toggling behavior like keyboard events.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ToggleLayer<T>() where T : UILayer
        {
            var layer = SearchLayer<T>();
            if ( !layer.GameObject.Active )
            {
                ShowLayer( layer );
            }
            else
            {
                ShowLastLayer();
            }
        }

        /// <summary>
        /// Can be used with toggling behavior like keyboard events.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ToggleLayer<T>( bool isVisible ) where T : UILayer
        {
            var layer = SearchLayer<T>();
            if ( !isVisible )
            {
                ShowLayer( layer );
            }
            else
            {
                ShowLastLayer();
            }
        }

        /// <summary>
        /// Can be used with toggling behavior like keyboard events.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ToggleLayer<T>( object data ) where T : UILayer
        {
            var layer = SearchLayer<T>();
            if ( !layer.GameObject.Active )
            {
                ShowLayerWithData( layer, data );
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
                ShowLayer( null, false );
            }
        }

        /// <summary>
        /// Goes back to the starting layer.
        /// Resets the layer history.
        /// </summary>
        public void ResetToStartLayer()
        {
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
                if ( layer == currentLayer )
                {
                    currentLayer = null;
                }
            }
        }

        private UILayer SearchLayer<T>() where T : UILayer
        {
            foreach ( var layer in UILayers )
            {
                if ( layer is T )
                {
                    return layer;
                }
            }
            return null;
        }
    }

}