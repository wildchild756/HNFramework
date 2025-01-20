using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphNode : HNGraphBaseNode
    {
        public JsonData NodeData => nodeData;

        public string NodeDataTypeName => nodeDataTypeName;

        // public Type NodeDataType => nodeDataType;

        public IReadOnlyList<string> InputPortGuids => inputPortGuids;

        public IReadOnlyList<string> OutputPortGuids => outputPortGuids;

        // public HNGraphData EditorData
        // {
        //     set { editorData = value; }
        // }


        [SerializeField]
        protected List<string> inputPortGuids;
        
        [SerializeField]
        protected List<string> outputPortGuids;

        [SerializeField]
        private JsonData nodeData;

        [SerializeField]
        private string nodeDataTypeName;

        // private Type nodeDataType;

        // private HNGraphData editorData;


        public HNGraphNode(string nodeDataTypeName)
        {
            this.nodeDataTypeName = nodeDataTypeName;

            inputPortGuids = new List<string>();
            outputPortGuids = new List<string>();
        }

        public void Initialize(Vector2 position, HNGraphData editorData)
        {
            base.Initialize(position);

            Assembly assembly = Assembly.Load(editorData.GraphRuntimeAssemblyName);
            if(assembly != null)
            {
                Type nodeDataType = assembly.GetType($"{editorData.GraphNodeDataNamespace}.{nodeDataTypeName}");
                if(nodeDataType == null)
                    return;

                JsonObject jsonObject = Activator.CreateInstance(nodeDataType) as JsonObject;
                if(jsonObject == null)
                    return;
                
                nodeData = new JsonData(jsonObject);
            }
        }

        public Type GetNodeDataType(HNGraphData editorData)
        {
            Assembly assembly = Assembly.Load(editorData.GraphRuntimeAssemblyName);
            if(assembly != null)
            {
                return assembly.GetType($"{editorData.GraphNodeDataNamespace}.{nodeDataTypeName}");
            }

            return null;
        }

        public override void AddInputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphNodePort)
                return;

            if(inputPortGuids.Contains(port.Guid))
                return;

            editorData.AddNodePort(port as HNGraphNodePort);
            inputPortGuids.Add(port.Guid);
        }

        public override void AddOutputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphNodePort)
                return;

            if(outputPortGuids.Contains(port.Guid))
                return;

            editorData.AddNodePort(port as HNGraphNodePort);
            outputPortGuids.Add(port.Guid);
        }

        public override void RemoveInputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            editorData.RemoveNodePort(port as HNGraphNodePort);
            inputPortGuids.Remove(port.Guid);
        }

        public override void RemoveOutputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            editorData.RemoveNodePort(port as HNGraphNodePort);
            outputPortGuids.Remove(port.Guid);
        }

        public HNGraphBasePort GetInputPort(HNGraphData editorData, string guid)
        {
            return editorData.GetNodePort(guid);
        }

        public HNGraphBasePort GetOutputPort(HNGraphData editorData, string guid)
        {
            return editorData.GetNodePort(guid);
        }

        public override void Dispose()
        {
            // base.Dispose();

            // foreach(var portGuid in inputPortGuids)
            // {
            //     editorData.GetNodePort(portGuid).Dispose();
            // }
            // foreach(var portGuid in outputPortGuids)
            // {
            //     editorData.GetNodePort(portGuid).Dispose();
            // }
        }

    }
}
