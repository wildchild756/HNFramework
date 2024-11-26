using System;
using System.Collections;
using System.Collections.Generic;
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
            {
                Debug.Log($"{nodes} does not contains guid {guid}.");
                return null;
            }

            return nodes[guid];
        }

        public HNGraphConnection GetConnection(string guid)
        {
            if(!connections.ContainsKey(guid))
            {
                Debug.Log($"{connections} does not contains guid {guid}.");
                return null;
            }

            return connections[guid];
        }

        public HNGraphGroup GetGroup(string guid)
        {
            if(groups.ContainsKey(guid))
            {
                Debug.Log($"{groups} does not contains guid {guid}.");
                return null;
            }

            return groups[guid];
        }

        public HNGraphStickyNote GetStickyNote(string guid)
        {
            if(!stickyNotes.ContainsKey(guid))
            {
                Debug.Log($"{stickyNotes} does not contains guid {guid}.");
                return null;
            }

            return stickyNotes[guid];
        }

        public HNGraphRelayNode GetRelayNode(string guid)
        {
            if(!relayNodes.ContainsKey(guid))
            {
                Debug.Log($"{relayNodes} does not contains guid {guid}.");
                return null;
            }

            return relayNodes[guid];
        }

        public void AddNode(HNGraphNode node)
        {
            if(nodes.ContainsValue(node))
            {
                Debug.Log($"{nodes} already contains node guid {node}.");
                return;
            }
            nodes.Add(node.Guid, node);
        }

        public void AddConnection(HNGraphConnection connection)
        {
            if(connections.ContainsValue(connection))
            {
                Debug.Log($"{connections} already contains connection guid {connection}.");
                return;
            }
            connections.Add(connection.Guid, connection);
        }

        public void AddGroup(HNGraphGroup group)
        {
            if(groups.ContainsValue(group))
            {
                Debug.Log($"{groups} already contains group guid {group}.");
                return;
            }
            groups.Add(group.Guid, group);
        }

        public void AddStickyNote(HNGraphStickyNote stickyNote)
        {
            if(stickyNotes.ContainsValue(stickyNote))
            {
                Debug.Log($"{stickyNotes} already contains stickyNote guid {stickyNote}.");
                return;
            }
            stickyNotes.Add(stickyNote.Guid, stickyNote);
        }

        public void AddRelayNode(HNGraphRelayNode relayNode)
        {
            if(relayNodes.ContainsValue(relayNode))
            {
                Debug.Log($"{relayNodes} already contains relayNode guid {relayNode}.");
                return;
            }
            relayNodes.Add(relayNode.Guid, relayNode);
        }

        public void RemoveNode(HNGraphNode node)
        {
            if(!nodes.ContainsValue(node))
            {
                Debug.Log($"{nodes} does not contains node guid {node}.");
                return; 
            }
            nodes.Remove(node.Guid);
            node.Dispose();
        }

        public void RemoveConnection(HNGraphConnection connection)
        {
            if(!connections.ContainsValue(connection))
            {
                Debug.Log($"{connections} does not contains connection guid {connection}.");
                return; 
            }
            connections.Remove(connection.Guid);
            connection.Dispose();
        }

        public void RemoveGroup(HNGraphGroup group)
        {
            if(!groups.ContainsValue(group))
            {
                Debug.Log($"{groups} does not contains group guid {group}.");
                return; 
            }
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
            {
                Debug.Log($"{stickyNotes} does not contains stickyNote guid {stickyNote}.");
                return; 
            }
            stickyNotes.Remove(stickyNote.Guid);
            stickyNote.Dispose();
        }

        public void RemoveRelayNode(HNGraphRelayNode relayNode)
        {
            if(!relayNodes.ContainsValue(relayNode))
            {
                Debug.Log($"{relayNodes} does not contains relayNode guid {relayNode}.");
                return; 
            }
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


    }

}


