using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphEdgeView : Edge
    {
        public HNGraphEdge ConnectionData
        {
            get { return connectionData; }
            set { connectionData = value; }
        }
        private HNGraphEdge connectionData;


        public HNGraphEdgeView() : base()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            
        }

    }
}
