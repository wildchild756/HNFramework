using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphEdge : IDisposable
    {
        public string Guid => guid;
        [SerializeField]
        private string guid;
        
        public HNGraphPort OutputPort => outputPort;
        [SerializeReference]
        private HNGraphPort outputPort;

        public HNGraphPort InputPort => inputPort;
        [SerializeReference]
        private HNGraphPort inputPort;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphEdge(HNGraphEditorData editorData, HNGraphPort outputPort, HNGraphPort inputPort)
        {
            this.editorData = editorData;
            this.outputPort = outputPort;
            this.inputPort = inputPort;
            outputPort.ConnectToEdge(this);
            inputPort.ConnectToEdge(this);
            
            OnCreate();
        }

        public virtual void OnCreate()
        {
            guid = HNGraphUtils.NewGuid();
        }

        public virtual void Dispose()
        {
            outputPort = null;
            inputPort = null;
        }

    }
}
