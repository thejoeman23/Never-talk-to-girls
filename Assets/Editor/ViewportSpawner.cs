using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ViewportSpawner
{
    // Choose your key here
    private static readonly KeyCode spawnKey = KeyCode.G; 

    static ViewportSpawner()
    {
        // Hook into the editor update loop
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        // Check for key press (only on KeyDown, not KeyUp)
        if (e.type == EventType.KeyDown && e.keyCode == spawnKey)
        {
            SpawnAtSceneCamera(sceneView);
            e.Use(); // mark event as used so it doesn’t leak
        }
    }

    private static void SpawnAtSceneCamera(SceneView sceneView)
    {
        // Get scene view camera
        Camera cam = sceneView.camera;
        if (cam == null) return;

        // Create object
        GameObject go = new GameObject("SpawnedEmpty");
        Undo.RegisterCreatedObjectUndo(go, "Spawn Empty at View");

        // Match position + rotation
        go.transform.position = cam.transform.position;
        go.transform.rotation = cam.transform.rotation;

        // Make it selected right away
        Selection.activeGameObject = go;
    }
}