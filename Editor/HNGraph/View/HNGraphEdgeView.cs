using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphEdgeView : Edge
    {
        public HNGraphEdge EdgeData
        {
            get { return edgeData; }
            set { edgeData = value; }
        }
        private HNGraphEdge edgeData;

        public HNGraphPortView OutputPortView
        {
            get { return (HNGraphPortView)output; }
            set { output = value; }
        }

        public HNGraphPortView InputPortView
        {
            get { return (HNGraphPortView)input; }
            set { input = value; }
        }

        private HNGraphView graphView;


        public HNGraphEdgeView(HNGraphView graphView)
        {
            this.graphView = graphView;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public HNGraphPortView FindAnotherPort(HNGraphPortView port)
        {
            if(OutputPortView == port)
            {
                return InputPortView;
            }
            else if(InputPortView == port)
            {
                return OutputPortView;
            }
            else
            {
                return null;
            }
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if(e.altKey)
            {
                graphView.CreateRelayNodeOnEdge(this, e.localMousePosition);
            }
        }

    }
}
