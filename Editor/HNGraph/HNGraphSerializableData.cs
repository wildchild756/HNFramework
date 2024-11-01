using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class SerializableGraphNodes : SerializedDictionary<string, HNGraphNode> {}
    [Serializable]
    public class SerializableGraphEdges : SerializedDictionary<string, HNGraphEdge> {}
    [Serializable]
    public class SerializableGraphPorts : SerializedDictionary<string, HNGraphPort> {}
}
