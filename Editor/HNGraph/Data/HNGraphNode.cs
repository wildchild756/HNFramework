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
        public Type NodeDataType => nodeDataType;

        [SerializeReference]
        private Type nodeDataType;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphNode(HNGraphEditorData editorData, Type nodeDataType)
        {
            this.editorData = editorData;
            this.nodeDataType = nodeDataType;
        }

        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);
        }

    }
}
