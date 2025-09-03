using System;
using System.Collections.Generic;
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
        var window = CreateInstance<DialogueGraphWindow>();
        window.titleContent = new GUIContent(graph.name);
        window.LoadGraph(graph);
        window.Show();
    }
    
    private void LoadGraph(DialogueGraph save)
    {
        _currentGraph = save;

        ConstructGraphView();
        GenerateToolbar();

        if (save.nodes.Count == 0)
            return;
        
        // Generate each node to its correct position
        foreach (var nodeData in save.nodes)
        {
            _graphView.CreateNode(nodeData);
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
                    
                    Edge newEdge = new Edge
                    {
                        input = input.inputContainer[0] as Port,
                        output = output.outputContainer[0] as Port
                    };

                    newEdge.output?.Connect(newEdge);
                    newEdge.input?.Connect(newEdge);

                    _graphView.AddElement(newEdge);
                }
                // If its a Prompt then loop through the responses and make the connections
                else if (nodeData is PromptNode promptNode)
                {
                    if (promptNode.Responses == null)
                        continue;
                    foreach (var response in promptNode.Responses)
                    {
                        Node output = node;
                        Node input = FindNodeByData(response);

                        if (input == null)
                            continue;
                        
                        Edge newEdge = new Edge
                        {
                            input = input.inputContainer[0] as Port,
                            output = output.outputContainer[0] as Port
                        };

                        newEdge.output?.Connect(newEdge);
                        newEdge.input?.Connect(newEdge);

                        _graphView.AddElement(newEdge);
                    }
                }
            }
        }
    }

    private void SaveGraph()
    {
        if (_currentGraph == null)
            return;
        
        List<DialogueNode> oldNodes = new List<DialogueNode>(_currentGraph.nodes);        
        _currentGraph.nodes.Clear();

        foreach (var node in _graphView.nodes.ToList())
        {
            if (node is not DialogueNodeView nodeView)
                continue;
            
            if (oldNodes.Contains(nodeView.Data))
            {
                oldNodes.Remove(nodeView.Data);
                
                nodeView.Data.Position = node.GetPosition().position;
                _currentGraph.nodes.Add(nodeView.Data);
            }
            else
            {
                nodeView.Data.Position = node.GetPosition().position;
                _currentGraph.nodes.Add(nodeView.Data);
                
                AssetDatabase.AddObjectToAsset(nodeView.Data, _currentGraph);
                EditorUtility.SetDirty(_currentGraph);
            }
        }

        foreach (var nodeData in oldNodes)
        {
            AssetDatabase.RemoveObjectFromAsset(nodeData);
            DestroyImmediate(nodeData, true);
        }
        
        AssetDatabase.SaveAssets();
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
            Debug.Log("Saved Successfully");
        });
        saveButton.text = "Save";
        toolbar.Add(saveButton);
        
        var promptNodeButton = new Button(() =>
        {
            _graphView.CreateNode(CreateNodeData(true));
        });
        promptNodeButton.text = "Add Prompt Node";
        toolbar.Add(promptNodeButton);
        
        var responseNodeButton = new Button(() =>
        {
            _graphView.CreateNode(CreateNodeData(false));
        });
        responseNodeButton.text = "Add Response Node";
        toolbar.Add(responseNodeButton);

        rootVisualElement.Add(toolbar);
    }

    private DialogueNode CreateNodeData(bool isPrompt)
    {
        DialogueNode nodeData = isPrompt ? ScriptableObject.CreateInstance<PromptNode>() : ScriptableObject.CreateInstance<ResponseNode>();
        nodeData.GUID = Guid.NewGuid().ToString();
        
        return nodeData;
    }
}