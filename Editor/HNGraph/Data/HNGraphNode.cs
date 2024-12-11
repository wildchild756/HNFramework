using System;
using System.Collections;
using System.Collections.Generic;
using HN.Serialize;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphNode : HNGraphBaseNode
    {
        public IHNGraphNode NodeData => nodeData;

        [SerializeReference]
        private IHNGraphNode nodeData;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphNode(HNGraphEditorData editorData, IHNGraphNode nodeData)
        {
            this.editorData = editorData;
            this.nodeData = nodeData;
        }

        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
        }

    }
}
