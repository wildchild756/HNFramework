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


        public HNGraphEdge(HNGraphPort outputPort, HNGraphPort inputPort)
        {
            guid = HNGraphUtils.NewGuid();
            
            this.outputPort = outputPort;
            outputPort.ConnectToEdge(this);
            this.inputPort = inputPort;
            inputPort.ConnectToEdge(this);
        }

        public void Dispose()
        {
            outputPort = null;
            inputPort = null;
        }

    }
}
