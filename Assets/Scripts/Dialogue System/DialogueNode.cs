using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode : BaseNode// A base class with base variables that i want all nodes to have
{
    public string Text;
    public AudioClip AudioClip;
    public List<DialogueNode> NextNodes = new List<DialogueNode>();
}