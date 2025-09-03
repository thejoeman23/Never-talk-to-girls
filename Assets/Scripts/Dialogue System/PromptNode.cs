using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Prompt Node", menuName = "Dialogue Nodes/Prompt Node")]
public class PromptNode : DialogueNode
{
    public List<ResponseNode> Responses =  new List<ResponseNode>();
}
