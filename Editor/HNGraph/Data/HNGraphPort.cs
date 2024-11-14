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
        [SerializeField]
        private string guid;
        
        public string PortTypeName
        {
            get { return portTypeName; }
            set { portTypeName = value; }
        }
        [SerializeField]
        private string portTypeName;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [SerializeField]
        private string name;

        public Direction PortDirection => portDirection;
        [SerializeField]
        private Direction portDirection;

        public Capacity PortCapacity => portCapacity;
        [SerializeField]
        private Capacity portCapacity;

        public HNGraphBaseNode OwnerNode => ownerNode;
        [SerializeReference]
        private HNGraphBaseNode ownerNode;

        public List<HNGraphEdge> Edges => edges;
        [SerializeReference]
        private List<HNGraphEdge> edges;


        public HNGraphPort(HNGraphBaseNode ownerNode, string typeName, string name, Direction direction, Capacity capacity)
        {
            guid = HNGraphUtils.NewGuid();
            
            this.ownerNode = ownerNode;
            this.portTypeName = typeName;
            this.name = name;
            this.portDirection = direction;
            this.portCapacity = capacity;
            
            edges = new List<HNGraphEdge>();
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

        public void ConnectToEdge(HNGraphEdge edge)
        {
            if(!edges.Contains(edge))
            {
                edges.Add(edge);
            }
        }

        public void DisconnectFromEdge(HNGraphEdge edge)
        {
            if(edges.Contains(edge))
            {
                edges.Remove(edge);
            }
        }

        public virtual void Dispose()
        {
            if(edges != null && edges.Count != 0)
            {
                for(int i = 0; i < edges.Count; i++)
                {
                    DisconnectFromEdge(edges[i]);
                }
            }
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
