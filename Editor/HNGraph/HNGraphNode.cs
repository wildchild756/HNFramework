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
        public IHNGraphNode GraphNodeClass => graphNodeClass;
        [SerializeReference]
        private IHNGraphNode graphNodeClass;

        public List<HNGraphPort> InputPorts => inputPorts;
        [SerializeField]
        private List<HNGraphPort> inputPorts;

        public List<HNGraphPort> OutputPorts => outputPorts;
        [SerializeField]
        private List<HNGraphPort> outputPorts;

        public Rect Position => position;
        [SerializeField]
        private Rect position;

        public HNGraphNode(IHNGraphNode graphNodeClass)
        {
            this.graphNodeClass = graphNodeClass;
            inputPorts = new List<HNGraphPort>();
            outputPorts = new List<HNGraphPort>();

        }

        public void AddPort(HNGraphPort port)
        {
            if(port == null)
            {
                return;
            }

            if(port.PortDirection == HNGraphPort.Direction.Input)
            {
                inputPorts.Add(port);
            }
            else if(port.PortDirection == HNGraphPort.Direction.Output)
            {
                outputPorts.Add(port);
            }
        }

        public HNGraphPort FindInputPortWithIdentifier(string identifier)
        {
            foreach(var inputPort in inputPorts)
            {
                if(inputPort.Identifier == identifier)
                {
                    return inputPort;
                }
            }

            return null;
        }

        public HNGraphPort FindOutputPortWithIdentifier(string identifier)
        {
            foreach(var outputPort in outputPorts)
            {
                if(outputPort.Identifier == identifier)
                {
                    return outputPort;
                }
            }

            return null;
        }

        public HNGraphPort FindPortWithIdentifier(string identifier)
        {
            HNGraphPort port = null;
            port = FindInputPortWithIdentifier(identifier);
            if(port == null)
            {
                port = FindOutputPortWithIdentifier(identifier);
            }

            return port;
        }

        public void SetPosition(Rect position)
        {
            this.position = position;
        }

        public void Dispose()
        {
            foreach(var port in inputPorts)
            {
                port.Dispose();
            }
            foreach(var port in outputPorts)
            {
                port.Dispose();
            }
        }


    }
}
