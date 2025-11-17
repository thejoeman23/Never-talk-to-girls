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
        scroll.style.height = 80;
        scroll.style.width = data is StartNode ? 100 : 300;
        scroll.style.backgroundColor = new StyleColor(Color.gray2);
        mainContainer.Add(scroll);

        RefreshExpandedState();
        RefreshPorts();
    }
    
    
}