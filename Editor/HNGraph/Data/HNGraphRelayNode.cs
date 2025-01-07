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
        public HNGraphRelayNodePort InputPort => inputPort;

        public HNGraphRelayNodePort OutputPort => outputPort;

        public HNGraphData EditorData
        {
            set { editorData = value; }
        }


        [SerializeField]
        protected HNGraphRelayNodePort inputPort;
        
        [SerializeField]
        protected HNGraphRelayNodePort outputPort;
        

        private HNGraphData editorData;


        public HNGraphRelayNode(HNGraphEdge edgeData)
        {

        }

        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
        }

        public override void AddInputPort(HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphRelayNodePort)
                return;

            inputPort = port as HNGraphRelayNodePort;
        }

        public override void AddOutputPort(HNGraphBasePort port)
        {
            if(port == null || port is not HNGraphRelayNodePort)
                return;

            outputPort = port as HNGraphRelayNodePort;
        }

        public override void Dispose()
        {
            base.Dispose();

            inputPort.Dispose();
            outputPort.Dispose();
        }
    }
}
