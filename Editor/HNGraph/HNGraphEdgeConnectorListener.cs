using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace HN.Graph.Editor
{
    public class HNGraphEdgeConnectorListener : IEdgeConnectorListener
    {
        public void OnDrop(GraphView graphView, Edge edge)
        {
            HNGraphEdgeView edgeView = (HNGraphEdgeView)edge;
            HNGraphView hnGraphView = (HNGraphView)graphView;
            if (edgeView == null || hnGraphView == null)
            {
                return;
            }

            if(edgeView.input == null || edgeView.output == null)
            {
                return;
            }

            if(edgeView.input.node == null || edgeView.output.node == null)
            {
                return;
            }
            hnGraphView.AddEdge(edgeView);
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            
        }
    }
}
