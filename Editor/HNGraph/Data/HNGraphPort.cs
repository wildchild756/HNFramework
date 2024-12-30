using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphPort : HNGraphBasePort
    {
        public HNGraphPort(HNGraphBaseNode ownerNode, HNGraphData editorData, string typeName, string name, Direction direction, Capacity capacity) : base(ownerNode, editorData, typeName, name, direction, capacity)
        {
        }
    }
}
