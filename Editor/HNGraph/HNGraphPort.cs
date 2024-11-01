using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphPort : IDisposable
    {
        public string Identifier => identifier;
        [SerializeField]
        private string identifier;
        
        public Type PortType
        {
            get { return portType; }
            set { portType = value; }
        }
        [SerializeField]
        private Type portType;

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

        public HNGraphNode OwnerNode => ownerNode;
        [SerializeReference]
        private HNGraphNode ownerNode;

        public List<HNGraphEdge> Edges;
        [SerializeReference]
        private List<HNGraphEdge> edges;

        public HNGraphPort(HNGraphNode ownerNode, Type type, string identifier, string name, Direction direction, Capacity capacity)
        {
            this.identifier = identifier;
            this.ownerNode = ownerNode;
            this.portType = type;
            this.name = name;
            this.portDirection = direction;
            this.portCapacity = capacity;
            
            edges = new List<HNGraphEdge>();
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

        public void Dispose()
        {
            foreach(var edge in edges)
            {
                DisconnectFromEdge(edge);
            }
        }


        public enum Direction
        {
            Input,
            Output
        }

        public enum Capacity
        {
            Single,
            Multi
        }
    }
}
