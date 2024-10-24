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
        public HNRenderPass pass;

        public string Guid => guid;
        [SerializeField]
        private string guid;

        public Rect Position => position;
        [SerializeField]
        private Rect position;

        public HNGraphNode(HNRenderPass pass)
        {
            this.pass = pass;
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
