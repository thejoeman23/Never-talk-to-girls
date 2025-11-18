using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode : BaseNode
{
    [TextArea(2,2)] public string Text;
    public AudioClip AudioClip;
    [HideInInspector] public List<BaseNode> NextNodes = new List<BaseNode>();
}