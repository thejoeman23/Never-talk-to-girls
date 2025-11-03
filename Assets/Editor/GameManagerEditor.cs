using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    private SerializedProperty menuSceneIndexProp;
    private SerializedProperty gameSceneIndexProp;
    
    private string[] sceneNames;

    private void OnEnable()
    {
        menuSceneIndexProp = serializedObject.FindProperty("menuSceneIndex");
        gameSceneIndexProp = serializedObject.FindProperty("gameSceneIndex");

        // Gather all scenes in Build Settings
        int sceneCount = EditorBuildSettings.scenes.Length;
        sceneNames = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            string path = EditorBuildSettings.scenes[i].path;
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            sceneNames[i] = name;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scene Configuration", EditorStyles.boldLabel);

        if (sceneNames.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found in Build Settings!", MessageType.Warning);
        }
        else
        {
            menuSceneIndexProp.intValue = EditorGUILayout.Popup("Menu Scene", menuSceneIndexProp.intValue, sceneNames);
            gameSceneIndexProp.intValue = EditorGUILayout.Popup("Game Scene", gameSceneIndexProp.intValue, sceneNames);
        }

        serializedObject.ApplyModifiedProperties();
    }
}