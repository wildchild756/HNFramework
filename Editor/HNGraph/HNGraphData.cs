using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HN.Serialize;
using UnityEditor;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public abstract partial class HNGraphData
    {
        public HNGraphObject GraphObject
        {
            get { return graphObject; }
            set { graphObject = value; }
        }

        public HNGraphDataWrapper Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public IReadOnlyDictionary<string, HNGraphNode> Nodes => nodes;
        public IReadOnlyDictionary<string, HNGraphEdge> Edges => edges;
        public IReadOnlyDictionary<string, HNGraphGroup> Groups => groups;
        public IReadOnlyDictionary<string, HNGraphStickyNote> StickyNotes => stickyNotes;
        public IReadOnlyDictionary<string, HNGraphRelayNode> RelayNodes => relayNodes;


        [SerializeReference]
        protected HNGraphObject graphObject;

        [SerializeField]
        private SerializableNodes nodes;

        [SerializeField]
        private SerializableEdges edges;

        [SerializeField]
        private SerializableGroups groups;

        [SerializeField]
        private SerializableStickyNotes stickyNotes;

        [SerializeField]
        private SerializableRelayNodes relayNodes;


        private string assetPath;
        private HNGraphDataWrapper owner;


        public HNGraphData()
        {
            nodes = new SerializableNodes();
            edges = new SerializableEdges();
            groups = new SerializableGroups();
            stickyNotes = new SerializableStickyNotes();
            relayNodes = new SerializableRelayNodes();
        }

        public virtual void SaveAsset()
        {
            EditorUtility.SetDirty(graphObject);
            AssetDatabase.SaveAssetIfDirty(graphObject);

            Serialize();
        }


        public void GetGraphObject<T>(string path) where T : HNGraphObject
        {
            if(graphObject == null)
            {
                graphObject = AssetDatabase.LoadAssetAtPath<T>(path);
            }
        }

        public virtual void Initialize(string assetPath)
        {
            this.assetPath = assetPath;

            var graphDataWrapper = ScriptableObject.CreateInstance<HNGraphDataWrapper>();
            graphDataWrapper.Initialize(this);
        }

        public bool Serialize()
        {
            string jsonText = SerializeToJson();
            return Json.WriteToDisk(assetPath, jsonText);
        }

        public void Deserialize()
        {
            if(string.IsNullOrEmpty(assetPath))
                return;
            if(!File.Exists(assetPath))
                return;
            
            DeserializeFromString(Json.ReadFromDisk(assetPath));
        }

        public string SerializeToJson()
        {            
            return Json.Serialize(this);
        }

        public void DeserializeFromString(string jsonString)
        {
            Json.DeserializeFromString(this, jsonString);
        }

        public HNGraphNode GetNode(string guid)
        {
            if(!nodes.ContainsKey(guid))
                return null;

            return nodes[guid];
        }

        public int GetNodeIndex(HNGraphNode node)
        {
            if(!nodes.ContainsValue(node))
                return -1;
            
            return nodes.Values.ToList().IndexOf(node);
        }

        public HNGraphEdge GetEdge(string guid)
        {
            if(!edges.ContainsKey(guid))
                return null;

            return edges[guid];
        }

        public HNGraphGroup GetGroup(string guid)
        {
            if(groups.ContainsKey(guid))
                return null;

            return groups[guid];
        }

        public HNGraphStickyNote GetStickyNote(string guid)
        {
            if(!stickyNotes.ContainsKey(guid))
                return null;

            return stickyNotes[guid];
        }

        public HNGraphRelayNode GetRelayNode(string guid)
        {
            if(!relayNodes.ContainsKey(guid))
                return null;

            return relayNodes[guid];
        }

         public virtual void AddNode(HNGraphNode node)
        {
            if(nodes.ContainsValue(node))
                return;

            nodes.Add(node.Guid, node);
        }

         public virtual void AddEdge(HNGraphEdge edge)
        {
            if(edges.ContainsValue(edge))
                return;

            edges.Add(edge.Guid, edge);
        }

         public virtual void AddGroup(HNGraphGroup group)
        {
            if(groups.ContainsValue(group))
                return;
                
            groups.Add(group.Guid, group);
        }

         public virtual void AddStickyNote(HNGraphStickyNote stickyNote)
        {
            if(stickyNotes.ContainsValue(stickyNote))
                return;
                
            stickyNotes.Add(stickyNote.Guid, stickyNote);
        }

         public virtual void AddRelayNode(HNGraphRelayNode relayNode)
        {
            if(relayNodes.ContainsValue(relayNode))
                return;
                
            relayNodes.Add(relayNode.Guid, relayNode);
        }

         public virtual void RemoveNode(HNGraphNode node)
        {
            if(!nodes.ContainsValue(node))
                return;
                
            nodes.Remove(node.Guid);
            node.Dispose();
        }

         public virtual void RemoveEdge(HNGraphEdge edge)
        {
            if(!edges.ContainsValue(edge))
                return;
                
            edges.Remove(edge.Guid);
            edge.Dispose();
        }

         public virtual void RemoveGroup(HNGraphGroup group)
        {
            if(!groups.ContainsValue(group))
                return;
                
            foreach(var nodeGuid in group.InnerNodeGuids)
            {
                if(!string.IsNullOrEmpty(nodeGuid))
                    break;

                HNGraphNode node = GetNode(nodeGuid);
                RemoveNode(node);
            }
            groups.Remove(group.Guid);
            group.Dispose();
        }

         public virtual void RemoveStickyNote(HNGraphStickyNote stickyNote)
        {
            if(!stickyNotes.ContainsValue(stickyNote))
                return;
                
            stickyNotes.Remove(stickyNote.Guid);
            stickyNote.Dispose();
        }

         public virtual void RemoveRelayNode(HNGraphRelayNode relayNode)
        {
            if(!relayNodes.ContainsValue(relayNode))
                return;
                
            relayNodes.Remove(relayNode.Guid);
            relayNode.Dispose();
        }

        public List<HNGraphNode> PackNodesFromOutput(HNGraphNode outputNode)
        {
            List<HNGraphNode> nodeList = new List<HNGraphNode>();
            if(!nodes.ContainsValue(outputNode))
                return nodeList;
            
            nodeList.Add(outputNode);
            nodeList = PackInputNodes(nodeList, outputNode);

            return nodeList;
        }

        public List<HNGraphNode> PackNodesFromInput(HNGraphNode inputNode)
        {
            List<HNGraphNode> nodeList = new List<HNGraphNode>();
            if(!nodes.ContainsValue(inputNode))
                return nodeList;
            
            nodeList.Add(inputNode);
            nodeList = PackOutputNodes(nodeList, inputNode);

            return nodeList;
        }

        public List<HNGraphNode> FindNodesWithType<T>() where T : class
        {
            List<HNGraphNode> list = new List<HNGraphNode>();

            foreach(var node in Nodes.Values)
            {
                if (node.NodeDataTypeName == null)
                    continue;
                
                if(node.NodeDataTypeName == typeof(T).Name)
                {
                    list.Add(node);
                }
            }
            
            return list;
        }
        

        private List<HNGraphNode> PackInputNodes(List<HNGraphNode> nodeList, HNGraphNode node)
        {
            List<HNGraphPort> inputPorts = node.InputPorts.Values.ToList();
            if(inputPorts.Count == 0)
                return nodeList;
            for(int i = inputPorts.Count - 1; i >= 0; i--)
            {
                List<HNGraphNode> connectedNodes = inputPorts[i].GetConnectedNodes(true, this);
                if(connectedNodes.Count == 0)
                    continue;
                for(int j = connectedNodes.Count - 1; j >= 0; j--)
                {
                    nodeList.Add(connectedNodes[j]);
                    PackInputNodes(nodeList, connectedNodes[j]);
                }
            }
            return nodeList;
        }

        private List<HNGraphNode> PackOutputNodes(List<HNGraphNode> nodeList, HNGraphNode node)
        {
            List<HNGraphPort> outputPorts = node.OutputPorts.Values.ToList();
            if(outputPorts.Count == 0)
                return nodeList;
            for(int i = outputPorts.Count - 1; i >= 0; i--)
            {
                List<HNGraphNode> connectedNodes = outputPorts[i].GetConnectedNodes(false, this);
                if(connectedNodes.Count == 0)
                    continue;
                for(int j = connectedNodes.Count - 1; j >= 0; j--)
                {
                    nodeList.Add(connectedNodes[j]);
                    PackOutputNodes(nodeList, connectedNodes[j]);
                }
            }
            return nodeList;
        }
    }

}


