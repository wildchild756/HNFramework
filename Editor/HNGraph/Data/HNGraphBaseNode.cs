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

        public IReadOnlyDictionary<string, HNGraphPort> InputPorts => inputPorts;

        public IReadOnlyDictionary<string, HNGraphPort> OutputPorts => outputPorts;


        [SerializeField]
        private string guid;

        [SerializeField]
        private SerializablePorts inputPorts;
        
        [SerializeField]
        private SerializablePorts outputPorts;

        [SerializeField]
        private Rect layout;


        public HNGraphBaseNode()
        {
            inputPorts = new SerializablePorts();
            outputPorts = new SerializablePorts();
        }

        public virtual void Initialize(Vector2 position)
        {
            guid = HNGraphUtils.NewGuid();
            SetLayout(new Rect(position, Vector2.zero));
        }

        public virtual void OnConnectionAdded(string portGuid)
        {

        }

        public void AddInputPort(HNGraphPort port)
        {
            if(port == null)
                return;

            if(inputPorts.ContainsKey(port.Guid))
            {
                // Debug.LogWarning($"Guid {port.Guid} already contained in {inputPorts}.");
                return;
            }
            
            inputPorts.Add(port.Guid, port);
        }

        public void AddOutputPort(HNGraphPort port)
        {
            if(port == null)
                return;

            if(outputPorts.ContainsKey(port.Guid))
            {
                // Debug.LogWarning($"Guid {port.Guid} already contained in {outputPorts}.");
                return;
            }
            
            outputPorts.Add(port.Guid, port);
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

        public HNGraphPort GetInputPort(string guid)
        {
            if(!inputPorts.ContainsKey(guid))
            {
                // Debug.LogWarning($"{inputPorts} has not contains Guid {guid}.");
                return null;
            }

            return inputPorts[guid];
        }

        public HNGraphPort GetOutputPort(string guid)
        {
            if(!outputPorts.ContainsKey(guid))
            {
                // Debug.LogWarning($"{outputPorts} has not contains Guid {guid}.");
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
