using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Prompt Node", menuName = "Dialogue Nodes/Prompt Node")]
public class PromptNode : DialogueNode
{
    [HideInInspector] public List<ResponseNode> Responses;
}
