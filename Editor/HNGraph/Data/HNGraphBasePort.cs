using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public abstract class HNGraphBasePort : IDisposable
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

        public IReadOnlyList<string> EdgeGuids => edgeGuids;

        public HNGraphPort RefPort
        {
            get { return refPort; }
            set { refPort = value; }
        }

        public HNGraphData EditorData
        {
            set { editorData = value; }
        }


        [SerializeField]
        protected string guid;

        [SerializeField]
        protected string portTypeName;

        [SerializeField]
        protected string name;

        [SerializeField]
        protected Direction portDirection;

        [SerializeField]
        protected Capacity portCapacity;

        [SerializeReference]
        protected HNGraphBaseNode ownerNode;

        [SerializeField]
        protected List<string> edgeGuids;
        
        protected HNGraphData editorData;

        [SerializeReference]
        protected HNGraphPort refPort;


        public HNGraphBasePort(HNGraphBaseNode ownerNode, string typeName, string name, Direction direction, Capacity capacity)
        {
            guid = HNGraphUtils.NewGuid();
            
            this.ownerNode = ownerNode;
            this.portTypeName = typeName;
            this.name = name;
            this.portDirection = direction;
            this.portCapacity = capacity;
            
            edgeGuids = new List<string>();
        }

        public bool IsMatchWithAttribute(Type type, HNGraphPortInfo portInfo)
        {
            bool b = true;
            b &= this.portTypeName == type.FullName;
            b &= this.name == portInfo.PortName;
            b &= this.PortDirection == (portInfo.PortDirection == HNGraphPortInfo.Direction.Input ? Direction.Input : Direction.Output);
            b &= this.portCapacity == (portInfo.PortCapacity == HNGraphPortInfo.Capacity.Single ? Capacity.Single : Capacity.Multi);
            return b;
        }

        public virtual void ConnectToEdge(HNGraphEdge edgeData)
        {
            if(edgeData == null)
                return;

            string edgeGuid = edgeData.Guid;
            if(edgeGuids.Contains(edgeGuid))
                return;

            edgeGuids.Add(edgeGuid);
        }

        public virtual void DisconnectFromEdge(HNGraphEdge edgeData)
        {
            if(edgeData == null)
                return;

            string edgeGuid = edgeData.Guid;
            if(!edgeGuids.Contains(edgeGuid))
                return;

            edgeGuids.Remove(edgeGuid);
        }

        public List<HNGraphNode> GetConnectedNodes(bool isInputPort)
        {
            
            List<HNGraphNode> connectedNodes = new List<HNGraphNode>();
            if(editorData == null || edgeGuids.Count == 0)
                return connectedNodes;
            
            for(int i = 0; i < edgeGuids.Count; i++)
            {
                HNGraphEdge edge = editorData.Edges[edgeGuids[i]];
                HNGraphBaseNode node = isInputPort ? edge.OutputPort.OwnerNode : edge.InputPort.OwnerNode;
                if(node == null)
                    continue;
                if(node is HNGraphNode)
                    connectedNodes.Add(node as HNGraphNode);
                else if(node is HNGraphRelayNode)
                    connectedNodes.Add(
                        isInputPort ? 
                        (node as HNGraphRelayNode).OutputPort.RefPort.OwnerNode as HNGraphNode :
                        (node as HNGraphRelayNode).InputPort.RefPort.OwnerNode as HNGraphNode
                        );
            }

            return connectedNodes;
        }

        public virtual void Dispose()
        {
            // connectionGuids.Clear();
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
