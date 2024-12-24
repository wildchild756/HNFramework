using System;
using System.Collections;
using System.Collections.Generic;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphNode : HNGraphBaseNode
    {
        public Type NodeDataType => nodeDataType;

        public IReadOnlyDictionary<string, HNGraphPort> InputPorts => inputPorts;

        public IReadOnlyDictionary<string, HNGraphPort> OutputPorts => outputPorts;


        [SerializeField]
        protected SerializablePorts inputPorts;
        
        [SerializeField]
        protected SerializablePorts outputPorts;

        [SerializeField]
        private Type nodeDataType;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphNode(HNGraphEditorData editorData, Type nodeDataType)
        {
            this.editorData = editorData;
            this.nodeDataType = nodeDataType;

            inputPorts = new SerializablePorts();
            outputPorts = new SerializablePorts();
        }

        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
        }

        public override void AddInputPort(HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphPort)
                return;

            if(inputPorts.ContainsKey(port.Guid))
                return;

            inputPorts.Add(port.Guid, port as HNGraphPort);
        }

        public override void AddOutputPort(HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphPort)
                return;

            if(outputPorts.ContainsKey(port.Guid))
                return;

            outputPorts.Add(port.Guid, port as HNGraphPort);
        }

        public void RemoveInputPort(string Guid)
        {
            if(inputPorts.ContainsKey(Guid))
                inputPorts.Remove(Guid);
        }

        public void RemoveOutputPort(string Guid)
        {
            if(outputPorts.ContainsKey(Guid))
                outputPorts.Remove(Guid);
        }

        public HNGraphBasePort GetInputPort(string guid)
        {
            if(!inputPorts.ContainsKey(guid))
                return null;

            return inputPorts[guid];
        }

        public HNGraphBasePort GetOutputPort(string guid)
        {
            if(!outputPorts.ContainsKey(guid))
                return null;

            return outputPorts[guid];
        }

        public override void Dispose()
        {
            base.Dispose();

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
