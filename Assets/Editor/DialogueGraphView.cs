using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        
        this.graphViewChanged = OnGraphViewChanged;
    }

    public DialogueNodeView CreateNode(string nodeName, bool isPrompt)
    {
        DialogueNode nodeData = isPrompt ? ScriptableObject.CreateInstance<PromptNode>() : ScriptableObject.CreateInstance<ResponseNode>();
        nodeData.GUID = Guid.NewGuid().ToString();

        var node = new DialogueNodeView(nodeData);

        node.SetPosition(new Rect(Vector2.zero, new Vector2(200, 150)));

        var inputPort = GeneratePort(node, Direction.Input);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);

        var outputPort = GeneratePort(node, Direction.Output, isPrompt ? Port.Capacity.Multi : Port.Capacity.Single);
        outputPort.portName = "Next";
        node.outputContainer.Add(outputPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        AddElement(node);
        return node;
    }
    
    public DialogueNodeView CreateNodeFromData(DialogueNode data)
    {
        var node = new DialogueNodeView(data);
        
        node.SetPosition(new Rect(data.Position, new Vector2(200, 150)));

        var inputPort = GeneratePort(node, Direction.Input);
        inputPort.portName = "Input";
        node.inputContainer.Add(inputPort);

        var outputPort = GeneratePort(node, Direction.Output, data is PromptNode ? Port.Capacity.Multi : Port.Capacity.Single);
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

            if (startPort.node is DialogueNodeView portNodeView && port.node is DialogueNodeView startNodeView)
            {
                if (startNodeView.Data is PromptNode && portNodeView.Data is PromptNode)
                    return;
                if (startNodeView.Data is ResponseNode && portNodeView.Data is ResponseNode)
                    return;
                if (startNodeView.Data.IsStart)
                    return;
            }

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
                var input = edge.input.node is DialogueNodeView inputNodeView ? inputNodeView.Data : null;
                var output = edge.output.node is DialogueNodeView outputNodeView ? outputNodeView.Data: null;
                
                if (output is PromptNode a && input is ResponseNode b)
                    a.Responses.Add(b);
                else if (output is ResponseNode c && input is PromptNode d)
                    c.NextPrompt = d;
            }
        }
        return graphViewChange;
    }
}