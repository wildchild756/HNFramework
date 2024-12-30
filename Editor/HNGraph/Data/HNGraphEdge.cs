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

        public HNGraphBasePort OutputPort => outputPort;

        public HNGraphBasePort InputPort => inputPort;


        [SerializeField]
        private string guid;

        [SerializeReference]
        private HNGraphBasePort outputPort;

        [SerializeReference]
        private HNGraphBasePort inputPort;
        
        [SerializeReference]
        private HNGraphData editorData;


        public HNGraphEdge(HNGraphData editorData, HNGraphBasePort outputPort, HNGraphBasePort inputPort)
        {
            this.editorData = editorData;
            this.outputPort = outputPort;
            this.inputPort = inputPort;
        }

        public virtual void Initialize()
        {
            guid = HNGraphUtils.NewGuid();
            outputPort?.ConnectToEdge(guid);
            inputPort?.ConnectToEdge(guid);
        }

        public virtual void Dispose()
        {
            outputPort = null;
            inputPort = null;
        }

    }
}
