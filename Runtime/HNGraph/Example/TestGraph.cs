using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Graph;
using HN.Serialize;
using UnityEngine;
using UnityEditor;

namespace HN.Graph.Example
{
    public class TestGraph : HNGraphObject
    {
        public const string TestGraphExtension = "testgraph";


        public TestGraph()
        {
            
        }

#if UNITY_EDITOR
        public override void Serialize()
        {
            Json.Serialize(this, AssetPath);
        }
#endif

    }
}
