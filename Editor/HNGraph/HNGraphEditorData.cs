using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HN.Graph.Editor
{
    public class HNGraphEditorData : ScriptableObject
    {
        public HNGraphObject graphData;
        public string graphAssetName;

        public List<HNGraphNode> Nodes => nodes;
        [SerializeReference]
        private List<HNGraphNode> nodes;

        public List<HNGraphEdge> Connection => connection;
        [SerializeReference]
        private List<HNGraphEdge> connection;


        public HNGraphEditorData()
        {
            nodes = new List<HNGraphNode>();
            connection = new List<HNGraphEdge>();
        }

        public void Initialize(HNGraphObject graphData)
        {
            this.graphData = graphData;
        }

    }
}


