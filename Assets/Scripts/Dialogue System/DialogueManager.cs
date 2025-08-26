using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public void StartDialogue(DialogueGraph node)
    {
        // Display UI here
        
        //DisplayNode(node);
    }

    private void DisplayNode(DialogueNode node)
    {
        Debug.Log("Dialogue: " + node.Text);

        node.Event?.Invoke();
        // Display text and audio

        if (node.IsEnd)
        {
            node.Event?.Invoke();
            EndDialogue();
            return;
        }
        
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
    }
    
    public void SelectOption(DialogueNode option)
    {
        // hide options
        
        DisplayNode(option);
    }
}
