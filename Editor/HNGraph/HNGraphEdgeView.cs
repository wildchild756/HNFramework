using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    }
}
