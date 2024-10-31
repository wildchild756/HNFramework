using System;
using System.Collections;
using System.Collections.Generic;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    public abstract class HNGraphEditorData : ScriptableObject
    {
        [SerializeReference]
        public HNGraphObject graphData;

        public List<HNGraphNode> Nodes => nodes;
        [SerializeField]
        private List<HNGraphNode> nodes;

        public List<HNGraphEdge> Edges => edges;
        [SerializeField]
        private List<HNGraphEdge> edges;


        public abstract void SaveAsset();
        public abstract void Compile();

        public HNGraphEditorData()
        {
            nodes = new List<HNGraphNode>();
            edges = new List<HNGraphEdge>();
        }

        public void Initialize(HNGraphObject graphData)
        {
            this.graphData = graphData;

            if(!string.IsNullOrEmpty(graphData.SerializedEditorData))
            {
                DeserializeEditorData(graphData.SerializedEditorData);
            }
            
        }

        public string SerializeEditorDataTo()
        {
            return Json.Serialize(this);
        }

        public void DeserializeEditorData(string serializeData)
        {
            Json.DeserializeFromString(this, serializeData);
        }

    }
}


