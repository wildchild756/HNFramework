using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphPort : HNGraphBasePort
    {
        public HNGraphPort(HNGraphBaseNode ownerNode, string typeName, string name, Direction direction, Capacity capacity) : base(ownerNode, typeName, name, direction, capacity)
        {
        }
    }
}
