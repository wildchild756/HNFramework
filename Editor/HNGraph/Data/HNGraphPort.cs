using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphPort : IDisposable
    {
        public string Guid => guid;

        public string PortTypeName
        {
            get { return portTypeName; }
            set { portTypeName = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Direction PortDirection => portDirection;

        public Capacity PortCapacity => portCapacity;

        public HNGraphBaseNode OwnerNode => ownerNode;

        public IReadOnlyList<string> ConnectionGuids => connectionGuids;


        [SerializeField]
        private string guid;

        [SerializeField]
        private string portTypeName;

        [SerializeField]
        private string name;

        [SerializeField]
        private Direction portDirection;

        [SerializeField]
        private Capacity portCapacity;

        [SerializeReference]
        private HNGraphBaseNode ownerNode;

        [SerializeReference]
        private List<string> connectionGuids;
        
        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphPort(HNGraphBaseNode ownerNode, HNGraphEditorData editorData, string typeName, string name, Direction direction, Capacity capacity)
        {
            guid = HNGraphUtils.NewGuid();
            
            this.ownerNode = ownerNode;
            this.editorData = editorData;
            this.portTypeName = typeName;
            this.name = name;
            this.portDirection = direction;
            this.portCapacity = capacity;
            
            connectionGuids = new List<string>();
        }

        public bool IsMatchWithAttribute(Type type, HNGraphPortInfoAttribute portInfo)
        {
            bool b = true;
            b &= this.portTypeName == type.FullName;
            b &= this.name == portInfo.PortName;
            b &= this.PortDirection == (portInfo.PortDirection == HNGraphPortInfoAttribute.Direction.Input ? Direction.Input : Direction.Output);
            b &= this.portCapacity == (portInfo.PortCapacity == HNGraphPortInfoAttribute.Capacity.Single ? Capacity.Single : Capacity.Multi);
            return b;
        }

        public void AddConnection(string Guid)
        {
            if(connectionGuids.Contains(Guid))
            {
                // Debug.LogWarning($"{connectionGuids} already contains connection guid {guid}.");
                return;
            }
            connectionGuids.Add(Guid);
        }

        public void RemoveConnection(string Guid)
        {
            if(!connectionGuids.Contains(Guid))
            {
                // Debug.LogWarning($"{connectionGuids} does not contains connection guid {guid}.");
                return;
            }
            connectionGuids.Remove(Guid);
        }

        public List<HNGraphNode> GetConnectedNodes()
        {
            List<HNGraphNode> connectedNodes = new List<HNGraphNode>();
            if(connectionGuids.Count == 0)
                return connectedNodes;
            
            for(int i = 0; i < connectionGuids.Count; i++)
            {
                HNGraphConnection connection = editorData.Connections[connectionGuids[i]];
                HNGraphNode node = connection.OutputPort.OwnerNode as HNGraphNode;
                if(node != null)
                {
                    connectedNodes.Add(node);
                }
            }

            return connectedNodes;
        }

        public virtual void Dispose()
        {
            connectionGuids.Clear();
        }


        [Serializable]
        public enum Direction
        {
            Input,
            Output
        }


        [Serializable]
        public enum Capacity
        {
            Single,
            Multi
        }
    }
}
