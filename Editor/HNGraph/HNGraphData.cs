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

        // public IReadOnlyDictionary<string, HNGraphNode> Nodes => nodes;
        // public IReadOnlyDictionary<string, HNGraphNodePort> NodePorts => nodePorts;
        // public IReadOnlyDictionary<string, HNGraphEdge> Edges => edges;
        // public IReadOnlyDictionary<string, HNGraphGroup> Groups => groups;
        // public IReadOnlyDictionary<string, HNGraphStickyNote> StickyNotes => stickyNotes;
        // public IReadOnlyDictionary<string, HNGraphRelayNode> RelayNodes => relayNodes;
        // public IReadOnlyDictionary<string, HNGraphRelayNodePort> RelayNodePorts => relayNodePorts;


        public Action<HNGraphNode> onNodeAdded;
        public Action<HNGraphNodePort> onNodePortAdded;
        public Action<HNGraphEdge> onEdgeAdded;
        public Action<HNGraphGroup> onGroupAdded;
        public Action<HNGraphStickyNote> onStickyNoteAdded;
        public Action<HNGraphRelayNode> onRelayNodeAdded;
        public Action<HNGraphRelayNodePort> onRelayNodePortAdded;
        public Action<HNGraphNode> onNodeRemoved;
        public Action<HNGraphNodePort> onNodePortRemoved;
        public Action<HNGraphEdge> onEdgeRemoved;
        public Action<HNGraphGroup> onGroupRemoved;
        public Action<HNGraphStickyNote> onStickyNoteRemoved;
        public Action<HNGraphRelayNode> onRelayNodeRemoved;
        public Action<HNGraphRelayNodePort> onRelayNodePortRemoved;


        [SerializeReference]
        protected HNGraphObject graphObject;

        [SerializeField]
        private SerializableNodes nodes;

        [SerializeField]
        private SerializableNodePorts nodePorts;

        [SerializeField]
        private SerializableEdges edges;

        [SerializeField]
        private SerializableGroups groups;

        [SerializeField]
        private SerializableStickyNotes stickyNotes;

        [SerializeField]
        private SerializableRelayNodes relayNodes;

        [SerializeField]
        private SerializableRelayNodePorts relayNodePorts;


        private string assetPath;
        private HNGraphDataWrapper owner;


        public HNGraphData()
        {
            nodes = new SerializableNodes();
            nodePorts = new SerializableNodePorts();
            edges = new SerializableEdges();
            groups = new SerializableGroups();
            stickyNotes = new SerializableStickyNotes();
            relayNodes = new SerializableRelayNodes();
            relayNodePorts = new SerializableRelayNodePorts();
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

        public HNGraphNodePort GetNodePort(string guid)
        {
            if(!nodePorts.ContainsKey(guid))
                return null;

            return nodePorts[guid];
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

        public HNGraphRelayNodePort GetRelayNodePort(string guid)
        {
            if(!relayNodePorts.ContainsKey(guid))
                return null;

            return relayNodePorts[guid];
        }

        public HNGraphBaseNode GetBaseNode(string guid)
        {
            if(nodes.ContainsKey(guid))
                return nodes[guid];
            
            if(relayNodes.ContainsKey(guid))
                return relayNodes[guid];
            
            return null;
        }

        public HNGraphBasePort GetBasePort(string guid)
        {
            if(nodePorts.ContainsKey(guid))
                return nodePorts[guid];
            
            if(relayNodePorts.ContainsKey(guid))
                return relayNodePorts[guid];
            
            return null;
        }

        public List<string> GetAllNodeGuids()
        {
            return nodes.Keys.ToList();
        }

        public List<string> GetAllNodePortGuids()
        {
            return nodePorts.Keys.ToList();
        }

        public List<string> GetAllEdgeGuids()
        {
            return edges.Keys.ToList();
        }

        public List<string> GetAllGroupGuids()
        {
            return groups.Keys.ToList();
        }

        public List<string> GetAllStickyNoteGuids()
        {
            return stickyNotes.Keys.ToList();
        }

        public List<string> GetAllRelayNodeGuids()
        {
            return relayNodes.Keys.ToList();
        }

        public List<string> GetAllRelayNodePortGuids()
        {
            return relayNodePorts.Keys.ToList();
        }

        public void AddNode(HNGraphNode node)
        {
            if(nodes.ContainsValue(node))
                return;

            onNodeAdded?.Invoke(node);
            nodes.Add(node.Guid, node);
        }

        public void AddNodePort(HNGraphNodePort nodePort)
        {
            if(nodePorts.ContainsValue(nodePort))
                return;

            nodePorts.Add(nodePort.Guid, nodePort);
        }

        public void AddEdge(HNGraphEdge edge)
        {
            if(edges.ContainsValue(edge))
                return;
            
            onEdgeAdded?.Invoke(edge);
            edges.Add(edge.Guid, edge);
        }

        public void AddGroup(HNGraphGroup group)
        {
            if(groups.ContainsValue(group))
                return;
            
            onGroupAdded?.Invoke(group);
            groups.Add(group.Guid, group);
        }

        public void AddStickyNote(HNGraphStickyNote stickyNote)
        {
            if(stickyNotes.ContainsValue(stickyNote))
                return;
            
            onStickyNoteAdded?.Invoke(stickyNote);
            stickyNotes.Add(stickyNote.Guid, stickyNote);
        }

        public void AddRelayNode(HNGraphRelayNode relayNode)
        {
            if(relayNodes.ContainsValue(relayNode))
                return;
            
            onRelayNodeAdded?.Invoke(relayNode);
            relayNodes.Add(relayNode.Guid, relayNode);
        }

        public void AddRelayNodePort(HNGraphRelayNodePort relayNodePort)
        {
            if(relayNodePorts.ContainsValue(relayNodePort))
                return;

            relayNodePorts.Add(relayNodePort.Guid, relayNodePort);
        }

        public void RemoveNode(HNGraphNode node)
        {
            if(!nodes.ContainsValue(node))
                return;
            
            onNodeRemoved?.Invoke(node);
            nodes.Remove(node.Guid);
            node.Dispose();
        }

        public void RemoveNodePort(HNGraphNodePort nodePort)
        {
            if(!nodePorts.ContainsValue(nodePort))
                return;
            
            onNodePortRemoved?.Invoke(nodePort);
            nodePorts.Remove(nodePort.Guid);
            nodePort.Dispose();
        }

        public void RemoveEdge(HNGraphEdge edge)
        {
            if(!edges.ContainsValue(edge))
                return;
            
            onEdgeRemoved?.Invoke(edge);
            edges.Remove(edge.Guid);
            edge.Dispose();
        }

        public void RemoveGroup(HNGraphGroup group)
        {
            if(!groups.ContainsValue(group))
                return;
            
            onGroupRemoved?.Invoke(group);
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

        public void RemoveStickyNote(HNGraphStickyNote stickyNote)
        {
            if(!stickyNotes.ContainsValue(stickyNote))
                return;
            
            onStickyNoteRemoved?.Invoke(stickyNote);
            stickyNotes.Remove(stickyNote.Guid);
            stickyNote.Dispose();
        }

        public void RemoveRelayNode(HNGraphRelayNode relayNode)
        {
            if(!relayNodes.ContainsValue(relayNode))
                return;
            
            onRelayNodeRemoved?.Invoke(relayNode);
            relayNodes.Remove(relayNode.Guid);
            relayNode.Dispose();
        }

        public void RemoveRelayNodePort(HNGraphRelayNodePort relayNodePort)
        {
            if(!relayNodePorts.ContainsValue(relayNodePort))
                return;
            
            onRelayNodePortRemoved?.Invoke(relayNodePort);
            relayNodePorts.Remove(relayNodePort.Guid);
            relayNodePort.Dispose();
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

            foreach(var node in nodes.Values)
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
            if(node.InputPortGuids.Count == 0)
                return nodeList;
            for(int i = node.InputPortGuids.Count - 1; i >= 0; i--)
            {
                string portGuid = node.InputPortGuids[i];
                List<HNGraphNode> connectedNodes = nodePorts[portGuid].GetConnectedNodes(true, this);
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
            if(node.OutputPortGuids.Count == 0)
                return nodeList;
            for(int i = node.OutputPortGuids.Count - 1; i >= 0; i--)
            {
                string portGuid = node.OutputPortGuids[i];
                List<HNGraphNode> connectedNodes = nodePorts[portGuid].GetConnectedNodes(false, this);
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


