using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Response Node", menuName = "Dialogue Nodes/Response Node")]
public class ResponseNode : DialogueNode
{
    public PromptNode NextPrompt;
}