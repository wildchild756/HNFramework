using System;
using System.Collections;
using System.Collections.Generic;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public abstract class HNGraphBaseNode : IDisposable, IPositionable, ISerializable
    {
        public string Guid => guid;
        [SerializeField]
        private string guid;

        public SerializablePorts InputPorts => inputPorts;
        [SerializeField]
        private SerializablePorts inputPorts;

        public SerializablePorts OutputPorts => outputPorts;
        [SerializeField]
        private SerializablePorts outputPorts;

        [SerializeField]
        private Rect layout;


        public HNGraphBaseNode()
        {
            inputPorts = new SerializablePorts();
            outputPorts = new SerializablePorts();
        }

        public virtual void OnCreate(Vector2 position)
        {
            guid = HNGraphUtils.NewGuid();
            SetLayout(new Rect(position, Vector2.zero));
        }

        public virtual void OnEdgeAdded(string portGuid)
        {

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

        public HNGraphPort GetInputPort(string guid)
        {
            if(!inputPorts.ContainsKey(guid))
            {
                return null;
            }

            return inputPorts[guid];
        }

        public HNGraphPort GetOutputPort(string guid)
        {
            if(!outputPorts.ContainsKey(guid))
            {
                return null;
            }

            return outputPorts[guid];
        }

        public Rect GetLayout()
        {
            return this.layout;
        }

        public void SetLayout(Rect layout)
        {
            this.layout = layout;
        }

        public string Serialize()
        {
            return Json.Serialize(this);
        }

        public void Deserialize(string serializeData)
        {
            Json.DeserializeFromString(this, serializeData);
        }

        public virtual void Dispose()
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
