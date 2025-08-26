using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public void StartDialogue(DialogueGraph dialogue)
    {
        // Display UI here

        foreach (var node in dialogue.nodes)
        {
            if (node.IsStart)
            {
                Debug.Log("Dialogue Start");
                DisplayNode(node);
                return;
            }
        }
        
        Debug.LogWarning("Dialogue start not found");
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
            
            SelectOption(prompt.Responses[0]);
        }
    }

    private void EndDialogue()
    {
        Debug.Log("Dialogue end");
        
        // Hide UI
    }
    
    public void SelectOption(DialogueNode option)
    {
        // hide options
        
        DisplayNode(option);
    }
}
