using Sandbox;

namespace Shooter;

/// <summary>
/// Is used to create Component singletons.
/// </summary>
/// <typeparam name="T">Type of inheriting class.</typeparam>
public abstract class SingletonBase<T> : Component where T : SingletonBase<T>
{
    [SkipHotload] public static T Instance { get; private set; } = null;

    protected override void OnAwake()
    {
        if ( Instance != null && Instance != this )
        {
            DestroyGameObject();
            return;
        }
        
        Instance = (T)this;
    }

    protected override void OnDestroy()
    {
        if ( Instance == this )
        {
            Instance = null;
        }
    }
}
