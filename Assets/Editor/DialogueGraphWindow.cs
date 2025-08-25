using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView _graphView;

    [MenuItem("Tools/Dialogue Graph")]
    public static void OpenDialogueGraph()
    {
        var window = GetWindow<DialogueGraphWindow>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
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

        var promptNodeButton = new Button(() =>
        {
            _graphView.CreateNode("New Prompt Node");
        });
        promptNodeButton.text = "Add Prompt Node";
        toolbar.Add(promptNodeButton);
        
        var responseNodeButton = new Button(() =>
        {
            _graphView.CreateNode("New Response Node");
        });
        responseNodeButton.text = "Add Response Node";
        toolbar.Add(responseNodeButton);

        rootVisualElement.Add(toolbar);
    }
}