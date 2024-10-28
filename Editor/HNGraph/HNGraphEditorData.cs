using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

            if(!string.IsNullOrEmpty(graphData.EditorDataJson))
            {
                DeserializeEditorDataFromJson(graphData.EditorDataJson);
            }
            
        }

        public string SerializeEditorDataToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void DeserializeEditorDataFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

    }
}


