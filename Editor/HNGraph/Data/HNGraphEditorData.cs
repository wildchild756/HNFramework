using System;
using System.Collections;
using System.Collections.Generic;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    public abstract class HNGraphEditorData : ScriptableObject, ISerializable
    {
        [SerializeReference]
        public HNGraphObject graphData;

        [SerializeField]
        private SerializableNodes nodes;

        [SerializeField]
        private SerializableEdges edges;

        [SerializeField]
        private SerializableGroups groups;

        [SerializeField]
        private SerializableStickyNotes stickyNotes;


        public abstract void SaveAsset();
        public abstract void Compile();

        public HNGraphEditorData()
        {
            nodes = new SerializableNodes();
            edges = new SerializableEdges();
            groups = new SerializableGroups();
            stickyNotes = new SerializableStickyNotes();
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
                return null;
            }

            return nodes[guid];
        }

        public HNGraphEdge GetEdge(string guid)
        {
            if(!edges.ContainsKey(guid))
            {
                return null;
            }

            return edges[guid];
        }

        public HNGraphGroup GetGroup(string guid)
        {
            if(groups.ContainsKey(guid))
            {
                return null;
            }

            return groups[guid];
        }

        public HNGraphStickyNote GetStickyNote(string guid)
        {
            if(!stickyNotes.ContainsKey(guid))
            {
                return null;
            }

            return stickyNotes[guid];
        }

        public IEnumerator GetNodesEnumerator()
        {
            return nodes.Values.GetEnumerator();
        }

        public IEnumerator GetEdgesEnumerator()
        {
            return edges.Values.GetEnumerator();
        }

        public IEnumerator GetGroupsEnumerator()
        {
            return groups.Values.GetEnumerator();
        }

        public IEnumerator GetStickyNoteEnumerator()
        {
            return stickyNotes.Values.GetEnumerator();
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

        public void AddGroup(HNGraphGroup group)
        {
            if(!groups.ContainsValue(group))
            {
                groups.Add(group.Guid, group);
            }
        }

        public void AddStickyNote(HNGraphStickyNote stickyNote)
        {
            if(!stickyNotes.ContainsValue(stickyNote))
            {
                stickyNotes.Add(stickyNote.Guid, stickyNote);
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

        public void RemoveGroup(HNGraphGroup group)
        {
            if(groups.ContainsValue(group))
            {
                foreach(var nodeGuid in group.InnerNodeGuids)
                {
                    if(!string.IsNullOrEmpty(nodeGuid))
                    {
                        break;
                    }

                    HNGraphNode node = GetNode(nodeGuid);
                    RemoveNode(node);
                }
                groups.Remove(group.Guid);
                group.Dispose();
            }
        }

        public void RemoveStickyNote(HNGraphStickyNote stickyNote)
        {
            if(stickyNotes.ContainsValue(stickyNote))
            {
                stickyNotes.Remove(stickyNote.Guid);
                stickyNote.Dispose();
            }
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


