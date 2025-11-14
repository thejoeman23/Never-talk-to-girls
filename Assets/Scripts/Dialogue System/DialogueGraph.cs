using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewDialogueTree", menuName = "Dialogue Tree")]
public class DialogueGraph : ScriptableObject
{
    public StartNode start;
    public List<BaseNode> nodes = new List<BaseNode>();
}