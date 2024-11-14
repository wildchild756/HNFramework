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

        public HNGraphNode(HNGraphEditorData editorData, IHNGraphNode graphNodeClass, Vector2 position) : base()
        {
            this.editorData = editorData;
            this.graphNodeClass = graphNodeClass;

            OnCreate(position);
        }

        public override void OnCreate(Vector2 position)
        {
            base.OnCreate(position);
        }

    }
}
