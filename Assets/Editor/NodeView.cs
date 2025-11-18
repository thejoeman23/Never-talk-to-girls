using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class NodeView : Node
{
    public string GUID;
    public BaseNode Data;

    public NodeView(BaseNode data)
    {
        Data = data;

        if (Data is EndNode)
            title =  "End";
        else if (Data is StartNode)
            title = "Start";
        else
            title = "Dialogue";
        
        // Create SerializedObject for binding
        var so = new SerializedObject(data);

        // Draw full inspector for this ScriptableObject inside the node
        var inspector = new InspectorElement(so);
        
        var scroll = new ScrollView();
        scroll.Add(inspector);
        SetDimensions(scroll);
        scroll.style.backgroundColor = new StyleColor(Color.gray2);
        mainContainer.Add(scroll);

        RefreshExpandedState();
        RefreshPorts();
    }

    // Sets dimensions of the node depending on what type of node it is
    private void SetDimensions(ScrollView scroll)
    {
        // Default settings
        int width = 300;
        int height = 100;

        if (Data is EndNode)
        {
            width = 300;
            height = 100;
        }
        else if (Data is StartNode)
        {
            width = 200;
            height = 100;
        }
        else if (Data is DialogueNode)
        {
            width = 300;
            height = 100;
        }

        scroll.style.height = height;
        scroll.style.width = width;
    }
}