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

        public SerializableGraphNodes Nodes => nodes;
        [SerializeField]
        private SerializableGraphNodes nodes;

        public SerializableGraphEdges Edges => edges;
        [SerializeField]
        private SerializableGraphEdges edges;


        public abstract void SaveAsset();
        public abstract void Compile();

        public HNGraphEditorData()
        {
            nodes = new SerializableGraphNodes();
            edges = new SerializableGraphEdges();
        }

        public void Initialize(HNGraphObject graphData)
        {
            this.graphData = graphData;

            if(!string.IsNullOrEmpty(graphData.SerializedEditorData))
            {
                DeserializeEditorData(graphData.SerializedEditorData);
            }
            
        }

        public void AddNode(HNGraphNode node)
        {
            if(!nodes.ContainsValue(node))
            {
                nodes.Add(node.Guid, node);
            }
        }

        public void AddEdge(HNGraphEdge edge)
        {
            if(!edges.ContainsValue(edge))
            {
                edges.Add(edge.Guid, edge);
            }
        }

        public void RemoveNode(HNGraphNode node)
        {
            if(nodes.ContainsValue(node))
            {

                nodes.Remove(node.Guid);
                node.Dispose();
            }
        }

        public void RemoveEdge(HNGraphEdge edge)
        {
            if(edges.ContainsValue(edge))
            {
                edges.Remove(edge.Guid);
                edge.Dispose();
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


