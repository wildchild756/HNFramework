using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HN.Serialize;

namespace HN.Graph.Editor
{
    public abstract partial class HNGraphData : JsonObject
    {
        public string GraphEditorAssemblyName
        {
            get { return graphEditorAssemblyName; }
            set { graphEditorAssemblyName = value; }
        }

        public string GraphRuntimeAssemblyName
        {
            get { return graphRuntimeAssemblyName; }
            set { graphRuntimeAssemblyName = value; }
        }

        public string GraphNodeDataNamespace
        {
            get { return graphNodeDataNamespace; }
            set { graphNodeDataNamespace = value; }
        }


        [SerializeField]
        private string graphEditorAssemblyName = "HN.Graph.Editor";

        [SerializeField]
        private string graphRuntimeAssemblyName = "HN.Graph";

        [SerializeField]
        private string graphNodeDataNamespace = "HN";
    }
}
