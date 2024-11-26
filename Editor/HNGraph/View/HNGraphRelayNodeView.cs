using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphRelayNodeView : HNGraphBaseNodeView
    {
        public HNGraphRelayNode RelayNodeData => BaseNodeData as HNGraphRelayNode;

        public HNGraphPortView InputPortView => InputPortViews[0];

        public HNGraphPortView OutputPortView => OutputPortViews[0];

        public int Index => index;


        private int index = 0;
    
    
        public HNGraphRelayNodeView(HNGraphView graphView, HNGraphRelayNode relayNodeData, HNGraphEdgeConnectorListener edgeConnectorListener)
         : base(graphView, relayNodeData, edgeConnectorListener)
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void DrawNode()
        {
            
        }

        protected override void DrawPorts()
        {
            HNGraphPort inputPortData = new HNGraphPort(
                RelayNodeData, 
                GraphView.GraphEditorData, 
                "", 
                "", 
                HNGraphPort.Direction.Input, 
                HNGraphPort.Capacity.Single
            );
            HNGraphPortView inputPortView = new HNGraphPortView(
                GraphView,
                inputPortData,
                this,
                "",
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Single,
                EdgeConnectorListener
            );
            AddPortView(inputPortView);
            RelayNodeData.AddInputPort(inputPortData);

            HNGraphPort outputPortData = new HNGraphPort(
                RelayNodeData, 
                GraphView.GraphEditorData, 
                "", 
                "", 
                HNGraphPort.Direction.Output, 
                HNGraphPort.Capacity.Single
            );
            HNGraphPortView outputPortView = new HNGraphPortView(
                GraphView,
                outputPortData,
                this,
                "",
                Orientation.Horizontal,
                Direction.Output,
                Port.Capacity.Single,
                EdgeConnectorListener
            );
            AddPortView(outputPortView);
            RelayNodeData.AddOutputPort(outputPortData);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if(e.altKey)
            {
                GraphView.DeleteRelayNode(this);
            }
        }
    }
}
