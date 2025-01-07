using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphNode : HNGraphBaseNode, ISerializationCallbackReceiver
    {
        public JsonData NodeData => nodeData;

        public string NodeDataTypeName => nodeDataTypeName;

        public Type NodeDataType
        {
            get
            {
                Assembly assembly = Assembly.Load(editorData.GraphRuntimeAssemblyName);
                Type nodeDataType = null;
                if(assembly != null)
                    nodeDataType =  assembly.GetType($"{editorData.GraphNodeDataNamespace}.{nodeDataTypeName}");
                return nodeDataType;
            }
        }

        public IReadOnlyDictionary<string, HNGraphPort> InputPorts => inputPorts;

        public IReadOnlyDictionary<string, HNGraphPort> OutputPorts => outputPorts;

        public HNGraphData EditorData
        {
            set { editorData = value; }
        }


        [SerializeField]
        protected SerializablePorts inputPorts;
        
        [SerializeField]
        protected SerializablePorts outputPorts;

        [SerializeField]
        private JsonData nodeData;

        [SerializeField]
        private string nodeDataTypeName;

        private Type nodeDataType;

        private HNGraphData editorData;


        public HNGraphNode(string nodeDataTypeName)
        {
            this.nodeDataTypeName = nodeDataTypeName;

            inputPorts = new SerializablePorts();
            outputPorts = new SerializablePorts();
            
            Assembly assembly = Assembly.Load(editorData.GraphRuntimeAssemblyName);
            if(assembly != null)
            {
                nodeDataType = assembly.GetType($"{editorData.GraphNodeDataNamespace}.{nodeDataTypeName}");
                if(nodeDataType == null)
                    return;

                JsonObject jsonObject = Activator.CreateInstance(nodeDataType) as JsonObject;
                if(jsonObject == null)
                    return;
                
                nodeData = new JsonData(jsonObject);
            }
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

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if(editorData == null)
                return;
            
            Assembly assembly = Assembly.Load(editorData.GraphRuntimeAssemblyName);
            if(assembly != null)
            {
                nodeDataType = assembly.GetType($"{editorData.GraphNodeDataNamespace}.{nodeDataTypeName}");
            }
        }
    }
}
