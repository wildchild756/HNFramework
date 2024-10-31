using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphPortView : Port
    {

        public HNGraphPortView(Direction portDirection, IEdgeConnectorListener connectListener, string name)
            : base(Orientation.Horizontal, portDirection, portDirection == Direction.Input ? Capacity.Single : Capacity.Multi, null)
        {
            var edgeConnector = new EdgeConnector<HNGraphEdgeView>(connectListener);
            this.AddManipulator(edgeConnector);
            portName = name;
            AddToClassList("port-view");
        }

        public bool IsComptibleWith(HNGraphPortView portView)
        {
            return portView != null
                && portView.node != node
                && portView.direction != direction
                && (!portView.connected || (portView.connected && portView.capacity == Capacity.Multi));
        }
    }
}
