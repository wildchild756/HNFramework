using System;
using System.Collections;
using System.Collections.Generic;
using HN;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphNode : IDisposable
    {
        public string Guid => guid;
        [SerializeField]
        private string guid;

        public IHNGraphNode GraphNodeClass => graphNodeClass;
        [SerializeReference]
        private IHNGraphNode graphNodeClass;

        public SerializableGraphPorts InputPorts => inputPorts;
        [SerializeField]
        private SerializableGraphPorts inputPorts;

        public SerializableGraphPorts OutputPorts => outputPorts;
        [SerializeField]
        private SerializableGraphPorts outputPorts;

        public Rect Position => position;
        [SerializeField]
        private Rect position;

        public HNGraphNode(IHNGraphNode graphNodeClass)
        {
            guid = HNGraphUtils.NewGuid();

            this.graphNodeClass = graphNodeClass;
            inputPorts = new SerializableGraphPorts();
            outputPorts = new SerializableGraphPorts();

        }

        public void AddPort(HNGraphPort port)
        {
            if(port == null)
            {
                return;
            }

            if(port.PortDirection == HNGraphPort.Direction.Input)
            {
                inputPorts.Add(port.Guid, port);
            }
            else if(port.PortDirection == HNGraphPort.Direction.Output)
            {
                outputPorts.Add(port.Guid, port);
            }
        }

        public void SetPosition(Rect position)
        {
            this.position = position;
        }

        public void Dispose()
        {
            foreach(var port in inputPorts.Values)
            {
                port.Dispose();
            }
            foreach(var port in outputPorts.Values)
            {
                port.Dispose();
            }
        }


    }
}
