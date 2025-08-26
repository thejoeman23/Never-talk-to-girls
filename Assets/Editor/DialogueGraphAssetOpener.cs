using UnityEditor;
using UnityEditor.Callbacks;

public static class DialogueGraphAssetOpener
{
    [OnOpenAsset(1)] // priority
    public static bool OnOpenAsset(int instanceID, int line)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);

        // If the clicked asset is our DialogueGraphSave, open the graph window
        if (obj is DialogueGraph graphSave)
        {
            DialogueGraphWindow.OpenGraph(graphSave);
            return true; // we handled it
        }

        return false; // let Unity handle other asset types normally
    }
}