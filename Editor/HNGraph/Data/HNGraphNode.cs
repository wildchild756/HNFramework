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
        public IHNGraphNode GraphNodeClass => graphNodeClass;

        [SerializeReference]
        private IHNGraphNode graphNodeClass;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphNode(HNGraphEditorData editorData, IHNGraphNode graphNodeClass)
        {
            this.editorData = editorData;
            this.graphNodeClass = graphNodeClass;
        }

        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
        }

    }
}
