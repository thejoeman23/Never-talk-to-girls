using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DialogueGraphView : GraphView
{
    public DialogueGraphView()
    {
        // Grid background
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // Allow zoom/drag
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }

    public DialogueNodeView CreateNode(string nodeName)
    {
        var nodeData = ScriptableObject.CreateInstance<DialogueNode>();
        nodeData.GUID = Guid.NewGuid().ToString();

        var node = new DialogueNodeView(nodeData);

        node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));

        var inputPort = GeneratePort(node, Direction.Input);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);

        var outputPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
        outputPort.portName = "Next";
        node.outputContainer.Add(outputPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        AddElement(node);
        return node;
    }

    private Port GeneratePort(DialogueNodeView node, Direction direction, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, direction, capacity, typeof(float));
    }
    
    // Tell GraphView which ports can connect
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if (startPort == port)
                return;

            if (startPort.node == port.node)
                return;

            if (startPort.direction == port.direction)
                return;

            compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // Optional: log edges being created
        if (graphViewChange.edgesToCreate != null)
        {
            foreach (var edge in graphViewChange.edgesToCreate)
            {
                Debug.Log($"Connected {edge.output.node.title} → {edge.input.node.title}");
            }
        }
        return graphViewChange;
    }
}