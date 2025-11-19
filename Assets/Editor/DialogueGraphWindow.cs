using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Toolbar = UnityEditor.UIElements.Toolbar;

public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView _graphView;
    private DialogueGraph _currentGraph;
    
    public static Dictionary<DialogueGraphWindow, DialogueGraph> GraphsOpen = new();
    
    public static void OpenGraph(DialogueGraph graph)
    {
        if (GraphsOpen.ContainsValue(graph))
        {
            var existingPair = GraphsOpen.FirstOrDefault(kv => kv.Value == graph);
            if (existingPair.Key != null)
            {
                existingPair.Key.Focus();
                return;
            }
            else
            {
                GraphsOpen.Remove(existingPair.Key);
            }
        }
        
        var window = CreateInstance<DialogueGraphWindow>();
        window.titleContent = new GUIContent(graph.name);
        window.LoadGraph(graph);
        window.Show();
        
        GraphsOpen.Add(window, graph);
    }
    
    // Load the graph from the provided save SO
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
        
        _graphView.SpawnNodeConnections();
    }
    
    // Save the graph data to the graph SO
    private void SaveGraph()
    {
        // If there's no graph loaded, do nothing
        if (_currentGraph == null)
            return;
    
        // Make a copy of the current nodes in the graph SO
        List<BaseNode> oldNodes = new List<BaseNode>(_currentGraph.nodes);        
        // Clear the graph SO's node list to rebuild it from the UI
        _currentGraph.nodes.Clear();

        // Loop through all nodes currently in the graph view/window
        foreach (var node in _graphView.nodes.ToList())
        {
            // Skip anything that's not a NodeView (our UI wrapper for DialogueNode)
            if (node is not NodeView nodeView)
                continue;
        
            // If this node has already been logged in the graph SO, update the position and re-add it to the graph SO
            if (oldNodes.Contains(nodeView.Data))
            {
                oldNodes.Remove(nodeView.Data); // Remove from the list so we know its been handled
                
                nodeView.Data.Position = node.GetPosition().position;
                _currentGraph.nodes.Add(nodeView.Data);
            }
            else
            {
                // If it's a new node, also store its UI position and add it to the graph SO
                nodeView.Data.Position = node.GetPosition().position;
                _currentGraph.nodes.Add(nodeView.Data);
            
                // Add the node as an asset in the graph so it gets saved with the graph
                AssetDatabase.AddObjectToAsset(nodeView.Data, _currentGraph);
                EditorUtility.SetDirty(_currentGraph); // Mark graph as modified
            }
        }

        // The remaining nodes that were in the SO but no longer exist in the actual graph view/window get deleted
        foreach (var nodeData in oldNodes)
        {
            AssetDatabase.RemoveObjectFromAsset(nodeData); // Remove from asset
            DestroyImmediate(nodeData, true); // Destroy the object
        }
    
        AssetDatabase.SaveAssets();
    }

    // No clue
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

        // Create save button
        var saveButton = new Button(() =>
        {
            SaveGraph();
            Debug.Log("Saved Successfully");
        });
        saveButton.text = "Save";
        toolbar.Add(saveButton);
        
        // Create start button
        var startNodeButton = new Button(() =>
        {
            StartNode newnode = CreateNodeData<StartNode>();
            _graphView.CreateNode(newnode);
            _currentGraph.start = newnode;
        });
        startNodeButton.text = "Add Start Node";
        toolbar.Add(startNodeButton);
        
        // Create dialogue button
        var dialogueNodeButton = new Button(() =>
        {
            _graphView.CreateNode(CreateNodeData<DialogueNode>());
        });
        dialogueNodeButton.text = "Add Dialogue Node";
        toolbar.Add(dialogueNodeButton);
        
        // Create end button
        var endNodeButton = new Button(() =>
        {
            _graphView.CreateNode(CreateNodeData<EndNode>());
        });
        endNodeButton.text = "Add End Node";
        toolbar.Add(endNodeButton);
        
        rootVisualElement.Add(toolbar);
    }

    // Creates a new node data depending on the class type you want
    private T CreateNodeData<T>() where T : BaseNode
    {
        T nodeData = CreateInstance<T>();
        nodeData.GUID = Guid.NewGuid().ToString();
        return nodeData;
    }
}