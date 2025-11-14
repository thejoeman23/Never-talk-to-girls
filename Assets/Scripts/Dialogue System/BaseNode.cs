using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "Base Node", menuName = "Dialogue Nodes/Base Node")]
public class BaseNode : ScriptableObject
{
    [HideInInspector] public Vector2 Position;
    [HideInInspector] public string GUID;
}
