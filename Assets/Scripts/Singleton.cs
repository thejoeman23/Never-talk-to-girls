using UnityEngine;

/// <summary>
/// Generic Singleton base class for Unity MonoBehaviours.
/// Ensures only one instance of the class exists and provides global access to it.
/// </summary>
/// <typeparam name="T">Type of the Singleton class.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// Static instance of the singleton.
    /// </summary>
    public static T Instance { get; protected set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Ensures that only one instance exists and initializes the singleton.
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // Destroy duplicate instance
            Destroy(gameObject);
            return;
        }

        // Assign the instance
        Instance = this as T;

        // Make the singleton persistent across scenes
        DontDestroyOnLoad(gameObject);

        // Call subclass's Awake logic
        OnAwake();
    }

    /// <summary>
    /// Override this method in subclasses instead of Awake for initialization.
    /// </summary>
    protected virtual void OnAwake()
    {
        // Optional override in subclasses
    }
}