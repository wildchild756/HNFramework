using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphNode
    {
        [SerializeReference]
        public IHNGraphNode graphNodeClass;

        public string Guid => guid;
        [SerializeField]
        private string guid;

        public Rect Position => position;
        [SerializeField]
        private Rect position;

        public HNGraphNode(IHNGraphNode graphNodeClass)
        {
            this.graphNodeClass = graphNodeClass;
            NewGUID();

        }

        public void SetPosition(Rect position)
        {
            this.position = position;
        }


        private void NewGUID()
        {
            guid = System.Guid.NewGuid().ToString();
        }


    }
}
