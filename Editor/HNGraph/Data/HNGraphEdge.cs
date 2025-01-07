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

        public HNGraphData EditorData
        {
            set { editorData = value; }
        }


        [SerializeField]
        private string guid;

        [SerializeReference]
        private HNGraphBasePort outputPort;

        [SerializeReference]
        private HNGraphBasePort inputPort;
        
        private HNGraphData editorData;


        public HNGraphEdge(HNGraphBasePort outputPort, HNGraphBasePort inputPort)
        {
            this.outputPort = outputPort;
            this.inputPort = inputPort;
        }

        public virtual void Initialize()
        {
            guid = HNGraphUtils.NewGuid();
            outputPort?.ConnectToEdge(this);
            inputPort?.ConnectToEdge(this);
        }

        public virtual void Dispose()
        {
            outputPort = null;
            inputPort = null;
        }

    }
}
