using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphEdge : IDisposable
    {
        public string Guid => guid;

        // public HNGraphBasePort OutputPort => editorData.GetBasePort(outputPortGuid);
        // public HNGraphBasePort InputPort => editorData.GetBasePort(inputPortGuid);
        public string OutputPortGuid => outputPortGuid;
        public string InputPortGuid => inputPortGuid;

        // public HNGraphData EditorData
        // {
        //     set { editorData = value; }
        // }


        [SerializeField]
        private string guid;

        [SerializeField]
        private string outputPortGuid;

        [SerializeField]
        private string inputPortGuid;
        
        // private HNGraphData editorData;


        public HNGraphEdge(HNGraphBasePort outputPort, HNGraphBasePort inputPort)
        {
            this.outputPortGuid = outputPort.Guid;
            this.inputPortGuid = inputPort.Guid;
        }

        public virtual void Initialize()
        {
            guid = HNGraphUtils.NewGuid();
            // editorData.GetBasePort(outputPortGuid)?.ConnectToEdge(this);
            // editorData.GetBasePort(inputPortGuid)?.ConnectToEdge(this);
        }

        public virtual void Dispose()
        {
            outputPortGuid = null;
            inputPortGuid = null;
        }

        public HNGraphBasePort GetOutputPort(HNGraphData editorData)
        {
            return editorData.GetBasePort(outputPortGuid);
        }

        public HNGraphBasePort GetInputPort(HNGraphData editorData)
        {
            return editorData.GetBasePort(inputPortGuid);
        }

    }
}
