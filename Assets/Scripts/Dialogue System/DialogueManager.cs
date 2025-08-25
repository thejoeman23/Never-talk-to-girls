using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public UnityEvent<Dialogue> _onDialogue = new UnityEvent<Dialogue>();
    public UnityEvent _onDialogueEnded = new UnityEvent();
    public UnityEvent<DialogueNode> _onOptionSelected = new UnityEvent<DialogueNode>();
    
    private void OnEnable()
    {
        _onDialogue.AddListener(StartDialogue);
        _onOptionSelected.AddListener(SelectOption);
    }

    private void OnDisable()
    {
        _onDialogue.RemoveListener(StartDialogue);
        _onOptionSelected.RemoveListener(SelectOption);
    }

    private void StartDialogue(Dialogue node)
    {
        // Display UI here
        
        DisplayNode(node.FirstNode);
    }

    private void DisplayNode(DialogueNode node)
    {
        if (node.IsEnd)
        {
            node.Event?.Invoke();
            EndDialogue();
            return;
        }

        node.Event?.Invoke();
        // Display text and audio

        if (node is ResponseNode response)
        {
            DisplayNode(response.NextPrompt);
        }
        else if (node is PromptNode prompt)
        {
            // Display options
        }
    }

    private void EndDialogue()
    {
        // Hide UI
        
        _onDialogueEnded.Invoke();
    }
    
    private void SelectOption(DialogueNode option)
    {
        // hide options
        
        DisplayNode(option);
    }
}
