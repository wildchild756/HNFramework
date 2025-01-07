using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphRelayNodePort : HNGraphBasePort
    {
        public HNGraphRelayNodePort(HNGraphBaseNode ownerNode, string typeName, string name, Direction direction, Capacity capacity) : base(ownerNode, typeName, name, direction, capacity)
        {
        }

    }
}
