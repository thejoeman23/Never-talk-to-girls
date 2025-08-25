using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogueNodeView : Node
{
    public string GUID;
    public DialogueNode Data;

    public DialogueNodeView(DialogueNode data)
    {
        Data = data;

        title = Data is PromptNode ? "Prompt" : "Response";
        
        // Create SerializedObject for binding
        var so = new SerializedObject(data);

        // Draw full inspector for this ScriptableObject inside the node
        var inspector = new InspectorElement(so);
        
        var scroll = new ScrollView();
        scroll.Add(inspector);
        scroll.style.height = 200; // or whatever
        scroll.style.width = 300;
        mainContainer.Add(scroll);

        RefreshExpandedState();
        RefreshPorts();
    }
}