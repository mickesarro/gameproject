using Sandbox;

/// <summary>
/// Is used to create Component singletons.
/// </summary>
/// <typeparam name="T">Type of inheriting class.</typeparam>
public abstract class SingletonBase<T> : Component where T : Component
{
    [SkipHotload] public static T Instance { get; private set; } = null;

    protected override void OnAwake()
    {
        if ( Instance != null && Instance != this && Application.IsEditor == false )
        {
            DestroyGameObject();
        }
        else
        {
            Instance = this as T;
        }
    }
}
