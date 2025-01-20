using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HN.Serialize;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphRelayNode : HNGraphBaseNode
    {
        // public HNGraphRelayNodePort InputPort => editorData.GetRelayNodePort(inputPortGuid);
        // public HNGraphRelayNodePort OutputPort => editorData.GetRelayNodePort(outputPortGuid);
        public string InputPortGuid => inputPortGuid;
        public string OutputPortGuid => outputPortGuid;

        // public HNGraphData EditorData
        // {
        //     set { editorData = value; }
        // }


        [SerializeField]
        protected string inputPortGuid = "";
        
        [SerializeField]
        protected string outputPortGuid = "";
        

        // private HNGraphData editorData;


        public HNGraphRelayNode(HNGraphEdge edgeData)
        {

        }

        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
        }

        public HNGraphRelayNodePort GetInputPort(HNGraphData editorData)
        {
            return editorData.GetRelayNodePort(inputPortGuid);
        }

        public HNGraphRelayNodePort GetOutputPort(HNGraphData editorData)
        {
            return editorData.GetRelayNodePort(outputPortGuid);
        }

        public override void AddInputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphRelayNodePort)
                return;
            
            editorData.AddRelayNodePort(port as HNGraphRelayNodePort);
            inputPortGuid = port.Guid;
        }

        public override void AddOutputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphRelayNodePort)
                return;

            editorData.AddRelayNodePort(port as HNGraphRelayNodePort);
            outputPortGuid = port.Guid;
        }

        public override void RemoveInputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            editorData.RemoveRelayNodePort(port as HNGraphRelayNodePort);
            inputPortGuid = "";
        }

        public override void RemoveOutputPort(HNGraphData editorData, HNGraphBasePort port)
        {
            editorData.RemoveRelayNodePort(port as HNGraphRelayNodePort);
            outputPortGuid = "";
        }

        public void SetRefPort(HNGraphData editorData, HNGraphNodePort refPortData)
        {
            if(refPortData == null)
            {
                editorData.GetRelayNodePort(inputPortGuid).RefPortGuid = "";
                editorData.GetRelayNodePort(outputPortGuid).RefPortGuid = "";
            }
            else
            {
                editorData.GetRelayNodePort(inputPortGuid).RefPortGuid = refPortData.Guid;
                editorData.GetRelayNodePort(outputPortGuid).RefPortGuid = refPortData.Guid;
            }
        }

        public override void Dispose()
        {
            // base.Dispose();

            // editorData.GetRelayNodePort(inputPortGuid).Dispose();
            // editorData.GetRelayNodePort(outputPortGuid).Dispose();
        }
    }
}
