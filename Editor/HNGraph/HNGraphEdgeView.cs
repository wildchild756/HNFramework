using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
    }
}
