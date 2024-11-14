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
        private HNGraphEdge edgeData;


        public HNGraphRelayNode(HNGraphEdge edgeData, Vector2 position) : base()
        {
            this.edgeData = edgeData;
            OnCreate(position);
        }

        public override void OnCreate(Vector2 position)
        {
            base.OnCreate(position);
        }

    }
}
