using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView _graphView;
    private DialogueGraph _currentGraph;

    public static void OpenGraph(DialogueGraph graph)
    {
        var window = GetWindow<DialogueGraphWindow>("Dialogue Graph");
        window.LoadGraph(graph);
    }
    
    private void LoadGraph(DialogueGraph save)
    {
        ConstructGraphView();
        GenerateToolbar();

        // Generate each node to its correct position
        foreach (var node in save.nodes)
        {
            _graphView.CreateNodeFromData(node);
        }

        // Create the connections
        foreach (var node in _graphView.nodes.ToList())
        {
            // Make sure the node we are looking at is a dialogue node
            if (node is DialogueNodeView nodeView)
            {
                var nodeData = nodeView.Data;
                
                // If it's a response node, find it's connection (if applicable) and create it
                if (nodeData is ResponseNode responseNode)
                {
                    Node output = node;
                    Node input = FindNodeByData(responseNode.NextPrompt);

                    if (input == null)
                        continue;
                    
                    Edge newEdge = new Edge();
                    newEdge.input = input.inputContainer[0] as Port;
                    newEdge.output = output.outputContainer[0] as Port;
                }
                // If its a Prompt then loop through the responses and make the connections
                else if (nodeData is PromptNode promptNode)
                {
                    foreach (var response in promptNode.Responses)
                    {
                        Node output = node;
                        Node input = FindNodeByData(response);

                        if (input == null)
                            continue;
                        
                        Edge newEdge = new Edge();
                        newEdge.input = input.inputContainer[0] as Port;
                        newEdge.output = output.outputContainer[0] as Port;
                    }
                }
            }
        }
        
        _currentGraph = save;
    }

    private void SaveGraph()
    {
        _currentGraph.nodes.Clear();

        foreach (var node in _graphView.nodes.ToList())
        {
            if (node is DialogueNodeView nodeView)
            {
                _currentGraph.nodes.Add(nodeView.Data);
            }
        }
    }

    private Node FindNodeByData(DialogueNode targetData)
    {
        if (targetData == null)
            return null;
        
        foreach (var node in _graphView.nodes.ToList())
        {
            if (node is DialogueNodeView nodeView && nodeView.Data == targetData)
                return node;
        }
        
        return null;
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var saveButton = new Button(() =>
        {
            SaveGraph();
        });
        saveButton.text = "Save";
        toolbar.Add(saveButton);
        
        var promptNodeButton = new Button(() =>
        {
            _graphView.CreateNode("New Prompt Node", true);
        });
        promptNodeButton.text = "Add Prompt Node";
        toolbar.Add(promptNodeButton);
        
        var responseNodeButton = new Button(() =>
        {
            _graphView.CreateNode("New Response Node", false);
        });
        responseNodeButton.text = "Add Response Node";
        toolbar.Add(responseNodeButton);

        rootVisualElement.Add(toolbar);
    }
}