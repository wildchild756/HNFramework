using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphConnection : IDisposable
    {
        public string Guid => guid;

        public HNGraphPort OutputPort => outputPort;

        public HNGraphPort InputPort => inputPort;

        public IReadOnlyList<string> RelayNodeGuids => relayNodeGuids;


        [SerializeField]
        private string guid;

        [SerializeReference]
        private HNGraphPort outputPort;

        [SerializeReference]
        private HNGraphPort inputPort;

        [SerializeField]
        private List<string> relayNodeGuids;
        
        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphConnection(HNGraphEditorData editorData, HNGraphPort outputPort, HNGraphPort inputPort)
        {
            this.editorData = editorData;
            this.outputPort = outputPort;
            this.inputPort = inputPort;
            relayNodeGuids = new List<string>();
        }

        public virtual void Initialize()
        {
            guid = HNGraphUtils.NewGuid();
            outputPort.AddConnection(guid);
            inputPort.AddConnection(guid);
        }

        public void AddRelayNode(int index, HNGraphRelayNode relayNode)
        {
            if(relayNode == null)
                return;

            if(relayNodeGuids.Contains(relayNode.Guid))
                throw new Exception($"{relayNodeGuids} already contains relay node guid {relayNode}");

            relayNodeGuids.Insert(index, relayNode.Guid);
        }

        public void RemoveRelayNode(HNGraphRelayNode relayNode)
        {
            if(relayNode == null)
                return;

            if(!relayNodeGuids.Contains(relayNode.Guid))
                // Debug.LogWarning($"{relayNodeGuids} does not contains relay node guid {relayNode}.");

            relayNodeGuids.Remove(relayNode.Guid);
        }

        public virtual void Dispose()
        {
            outputPort = null;
            inputPort = null;
        }

    }
}
