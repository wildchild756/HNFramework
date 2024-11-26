using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HN.Serialize;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphRelayNode : HNGraphBaseNode
    {
        public HNGraphPort InputPort => InputPorts.Values.ToList()[0];

        public HNGraphPort OutputPort => OutputPorts.Values.ToList()[0];
        

        [SerializeReference]
        private HNGraphConnection connectionData;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphRelayNode(HNGraphEditorData editorData, HNGraphConnection edgeData)
        {
            this.editorData = editorData;
            this.connectionData = edgeData;
        }

        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
        }

    }
}
