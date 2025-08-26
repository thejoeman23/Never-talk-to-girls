using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueTree", menuName = "Dialogue Tree")]
public class DialogueGraph : ScriptableObject
{ 
    public List<DialogueNode> nodes = new List<DialogueNode>();
}