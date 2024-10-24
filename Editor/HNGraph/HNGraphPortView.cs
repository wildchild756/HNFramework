using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HN.Graph.Editor
{
    public class HNGraphPortView : Port
    {

        public HNGraphPortView(Direction portDirection)
            : base(Orientation.Horizontal, portDirection, portDirection == Direction.Input ? Capacity.Single : Capacity.Multi, null)
        {

        }

        public bool IsComptibleWith(HNGraphPortView portView)
        {
            return portView != null
                && portView.node != node
                && portView.direction != direction;
        }
    }
}
