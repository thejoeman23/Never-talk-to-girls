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
        
        this.graphViewChanged = OnGraphViewChanged;

        // this.style.backgroundColor = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
    }
    
    // Spawns a node on the graph
    public NodeView CreateNode(BaseNode data)
    {
        if (data == null)
        {
            Debug.LogError("Tried to spawn node, but BaseNode was null.");
            return null;
        }
        
        var node = new NodeView(data);
        
        node.SetPosition(new Rect(data.Position, new Vector2(200, 150)));
        
        // Create an input port for Dialogue and End Nodes
        if (data is DialogueNode || data is EndNode)
        {
            var inputPort = GeneratePort(node, Direction.Input);
            inputPort.portName = "Input";
            node.inputContainer.Add(inputPort);
        }

        // Create an output port for Dialogue and Start Nodes
        if (data is DialogueNode || data is StartNode)
        {
            var outputPort = GeneratePort(node, Direction.Output);
            outputPort.portName = "Output";
            node.outputContainer.Add(outputPort);
        }

        node.RefreshExpandedState();
        node.RefreshPorts();
        
        // Adds node to graph
        AddElement(node);
        return node;
    }

    public void SpawnNodeConnections()
    {
        foreach (var node in nodes.ToList())
        {
            if (node is not NodeView nodeView)
                continue;

            var nodeData = nodeView.Data;

            if (nodeData is EndNode)
                continue;

            // Check if input port exists
            if (nodeView.inputContainer.childCount == 0)
                continue;
            
            var inputPort = nodeView.inputContainer[0] as Port;
            if (inputPort == null)
                continue;

            List<Node> outputs = GetOutputsOfNodeData(nodeData);

            foreach (var output in outputs)
            {
                if (output is not NodeView outputNodeView)
                    continue;

                // Check if output port exists
                if (outputNodeView.outputContainer.childCount == 0)
                    continue;
                
                var outputPort = outputNodeView.outputContainer[0] as Port;
                if (outputPort == null)
                    continue;

                Edge newEdge = new Edge
                {
                    input = inputPort,
                    output = outputPort
                };

                newEdge.output?.Connect(newEdge);
                newEdge.input?.Connect(newEdge);

                AddElement(newEdge);
            }
        }
    }
    
    // Returns a specific node
    private Node FindNodeByData(BaseNode targetData)
    {
        if (targetData == null)
            return null;
        
        foreach (var node in nodes.ToList())
        {
            if (node is NodeView nodeView && nodeView.Data == targetData)
                return node;
        }
        
        return null;
    }

    // Returns a list of the next nodes of a node.
    private List<Node> GetOutputsOfNodeData(BaseNode nodeData)
    {
        List<Node> outputs = new List<Node>();
        
        if (nodeData is StartNode startNode)
            foreach (var targetData in startNode.NextNodes)
                outputs.Add(FindNodeByData(targetData));
        else if (nodeData is DialogueNode dialogueNode)
            foreach (var targetData in dialogueNode.NextNodes)
                outputs.Add(FindNodeByData(targetData));

        return outputs;
    }
    
    // Creates port on a node
    private Port GeneratePort(NodeView node, Direction direction, Port.Capacity capacity = Port.Capacity.Multi)
    {
        return node.InstantiatePort(Orientation.Vertical, direction, capacity, typeof(float));
    }
    
    // Function takes in a node's port and returns a list of other ports that it can connect to
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
        if (graphViewChange.edgesToCreate == null)
            return graphViewChange;
        
        foreach (var edge in graphViewChange.edgesToCreate)
        {
            // start (the node the edge came FROM)
            var start = (edge.output.node as NodeView)?.Data;

            // end (the node the edge goes TO)
            var end = (edge.input.node as NodeView)?.Data;

            if (start == null || end == null)
                continue;

            if (start is DialogueNode dn)
                dn.NextNodes.Add(end);
            else if (start is StartNode sn)
                sn.NextNodes.Add(end);
            
            Debug.Log("Nodes Know theyre connected");
        }
        
        return graphViewChange;
    }
}