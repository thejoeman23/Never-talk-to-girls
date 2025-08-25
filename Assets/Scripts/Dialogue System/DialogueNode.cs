using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueNode : ScriptableObject// A base class with base variables that i want all nodes to have
{
    public string Text;
    public AudioSource Audio;
    public UnityEvent Event;
    public bool IsEnd;
}