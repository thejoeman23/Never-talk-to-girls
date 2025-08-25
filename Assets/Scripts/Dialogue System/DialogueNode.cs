using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueNode // A base class with base variables that i want all nodes to have
{
    public string Text;
    public AudioSource Audio;
    public UnityEvent Event;
    public bool IsEnd;
}

[System.Serializable]
public class PromptNode : DialogueNode // Said by the girl. Provides a list of responses at the end.
{
    public List<ResponseNode> Responses;
}

[System.Serializable]
public class ResponseNode : DialogueNode // Response from the player.
{
    public PromptNode NextPrompt;
}