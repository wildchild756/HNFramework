using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HN.Serialize;
using UnityEditor;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public abstract partial class HNGraphEditorData : ScriptableObject, ISerializable
    {
        public HNGraphObject GraphObject
        {
            get { return graphObject; }
            set { graphObject = value; }
        }

        public IReadOnlyDictionary<string, HNGraphNode> Nodes => nodes;
        public IReadOnlyDictionary<string, HNGraphEdge> Edges => edges;
        public IReadOnlyDictionary<string, HNGraphGroup> Groups => groups;
        public IReadOnlyDictionary<string, HNGraphStickyNote> StickyNotes => stickyNotes;
        public IReadOnlyDictionary<string, HNGraphRelayNode> RelayNodes => relayNodes;


        [SerializeReference]
        private HNGraphObject graphObject;

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


        public virtual void SaveAsset()
        {
            GraphObject.SerializedEditorData = Serialize();
            GraphObject.Serialize();
            
            EditorUtility.SetDirty(GraphObject);
            AssetDatabase.SaveAssets();
        }
        
        public void OnEnable()
        {
            nodes = new SerializableNodes();
            edges = new SerializableEdges();
            groups = new SerializableGroups();
            stickyNotes = new SerializableStickyNotes();
            relayNodes = new SerializableRelayNodes();
        }

        public virtual void Initialize(HNGraphObject graphData)
        {
            this.graphObject = graphData;

            if(!string.IsNullOrEmpty(graphData.SerializedEditorData))
            {
                Deserialize(graphData.SerializedEditorData);
            }
            
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

        public string Serialize()
        {
            return Json.Serialize(this);
        }

        public void Deserialize(string serializeData)
        {
            Json.DeserializeFromString(this, serializeData);
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
                if (node.NodeDataType == null)
                    continue;
                
                if(node.NodeDataType == typeof(T))
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
                List<HNGraphNode> connectedNodes = inputPorts[i].GetConnectedNodes(true);
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
                List<HNGraphNode> connectedNodes = outputPorts[i].GetConnectedNodes(false);
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


