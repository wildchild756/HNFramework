using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphNodePortView : HNGraphBasePortView
    {
        public new HNGraphNodePort PortData => portData as HNGraphNodePort;


        public HNGraphNodePortView(
            HNGraphView graphView, 
            HNGraphBasePort portData, 
            HNGraphBaseNodeView nodeView, 
            string name, 
            Orientation orientation, 
            Direction portDirection, 
            Capacity capacity, 
            IEdgeConnectorListener connectListener
            ) 
        : base(graphView, portData, nodeView, name, orientation, portDirection, capacity, connectListener)
        {
            
        }
    }
}
