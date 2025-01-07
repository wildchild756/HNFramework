using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphPortView : HNGraphBasePortView
    {
        public new HNGraphPort PortData => portData as HNGraphPort;


        public HNGraphPortView(
            HNGraphView graphView, 
            HNGraphBasePort portData, 
            HNGraphBaseNodeView nodeView, 
            string name, 
            Orientation orientation, 
            Direction portDirection, 
            Capacity capacity, 
            IEdgeConnectorListener connectListener,
            HNGraphBasePortView portView
            ) 
        : base(graphView, portData, nodeView, name, orientation, portDirection, capacity, connectListener, portView)
        {
            PortData.EditorData = graphView.GraphEditorData;
        }
    }
}
