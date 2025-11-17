using System.Collections.Generic;
using UnityEngine;

public class StartNode : BaseNode
{
    [HideInInspector] public List<BaseNode> NextNodes = new List<BaseNode>();
}