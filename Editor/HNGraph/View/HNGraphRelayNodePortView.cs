using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphRelayNodePortView : HNGraphBasePortView
    {
        public new HNGraphRelayNodePort PortData => portData as HNGraphRelayNodePort;


        public HNGraphRelayNodePortView(
            HNGraphView graphView, 
            HNGraphBasePort portData, 
            HNGraphBaseNodeView nodeView, 
            string name, 
            Orientation orientation, 
            Direction portDirection, 
            Capacity capacity, 
            IEdgeConnectorListener connectListener
            ) : base(graphView, portData, nodeView, name, orientation, portDirection, capacity, connectListener)
        {
            PortData.EditorData = graphView.GraphEditorData;

            // this.refPortView = (portView as HNGraphRelayNodePortView).RefPortView;
            // PortData.RefPort = (portView as HNGraphRelayNodePortView).RefPortView.PortData;
        }

        public void UpdateRefPortView(HNGraphPortView newPortView)
        {
            refPortView = newPortView;
            PortData.RefPort = newPortView?.PortData;
        }
    }
}
