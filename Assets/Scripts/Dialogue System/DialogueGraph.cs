using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDialogueTree", menuName = "Dialogue Tree")]
public class DialogueGraph : ScriptableObject
{ 
    public List<DialogueNode> nodes = new List<DialogueNode>();
}