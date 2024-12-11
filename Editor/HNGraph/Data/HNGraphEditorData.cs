using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    public abstract class HNGraphEditorData : ScriptableObject, ISerializable
    {
        public HNGraphObject GraphData
        {
            get { return graphData; }
            set { graphData = value; }
        }

        public IReadOnlyDictionary<string, HNGraphNode> Nodes => nodes;

        public IReadOnlyDictionary<string, HNGraphConnection> Connections => connections;

        public IReadOnlyDictionary<string, HNGraphGroup> Groups => groups;

        public IReadOnlyDictionary<string, HNGraphStickyNote> StickyNotes => stickyNotes;

        public IReadOnlyDictionary<string, HNGraphRelayNode> RelayNodes => relayNodes;


        [SerializeReference]
        private HNGraphObject graphData;

        [SerializeField]
        private SerializableNodes nodes;

        [SerializeField]
        private SerializableConnections connections;

        [SerializeField]
        private SerializableGroups groups;

        [SerializeField]
        private SerializableStickyNotes stickyNotes;

        [SerializeField]
        private SerializableRelayNodes relayNodes;


        public virtual void SaveAsset()
        {
            GraphData.SerializedEditorData = Serialize();
            GraphData.Serialize();
        }
        
        public HNGraphEditorData()
        {
            nodes = new SerializableNodes();
            connections = new SerializableConnections();
            groups = new SerializableGroups();
            stickyNotes = new SerializableStickyNotes();
            relayNodes = new SerializableRelayNodes();
        }

        public void Initialize(HNGraphObject graphData)
        {
            this.graphData = graphData;

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

        public HNGraphConnection GetConnection(string guid)
        {
            if(!connections.ContainsKey(guid))
                return null;

            return connections[guid];
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

        public void AddNode(HNGraphNode node)
        {
            if(nodes.ContainsValue(node))
                return;

            nodes.Add(node.Guid, node);
        }

        public void AddConnection(HNGraphConnection connection)
        {
            if(connections.ContainsValue(connection))
                return;

            connections.Add(connection.Guid, connection);
        }

        public void AddGroup(HNGraphGroup group)
        {
            if(groups.ContainsValue(group))
                return;
                
            groups.Add(group.Guid, group);
        }

        public void AddStickyNote(HNGraphStickyNote stickyNote)
        {
            if(stickyNotes.ContainsValue(stickyNote))
                return;
                
            stickyNotes.Add(stickyNote.Guid, stickyNote);
        }

        public void AddRelayNode(HNGraphRelayNode relayNode)
        {
            if(relayNodes.ContainsValue(relayNode))
                return;
                
            relayNodes.Add(relayNode.Guid, relayNode);
        }

        public void RemoveNode(HNGraphNode node)
        {
            if(!nodes.ContainsValue(node))
                return;
                
            nodes.Remove(node.Guid);
            node.Dispose();
        }

        public void RemoveConnection(HNGraphConnection connection)
        {
            if(!connections.ContainsValue(connection))
                return;
                
            connections.Remove(connection.Guid);
            connection.Dispose();
        }

        public void RemoveGroup(HNGraphGroup group)
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

        public void RemoveStickyNote(HNGraphStickyNote stickyNote)
        {
            if(!stickyNotes.ContainsValue(stickyNote))
                return;
                
            stickyNotes.Remove(stickyNote.Guid);
            stickyNote.Dispose();
        }

        public void RemoveRelayNode(HNGraphRelayNode relayNode)
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
                if (node.NodeData == null)
                    continue;
                
                T nodeData = node.NodeData as T;
                if (nodeData == null)
                    continue;
                
                list.Add(node);
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
                List<HNGraphNode> connectedNodes = inputPorts[i].GetConnectedNodes();
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
                List<HNGraphNode> connectedNodes = outputPorts[i].GetConnectedNodes();
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


