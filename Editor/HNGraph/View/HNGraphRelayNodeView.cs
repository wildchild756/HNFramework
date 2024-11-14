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

        public HNGraphEdge EdgeView => edgeView;
        private HNGraphEdge edgeView;

        private Type portType;
        
    
    
        public HNGraphRelayNodeView(HNGraphView graphView, HNGraphRelayNode relayNodeData, Type type, HNGraphEdgeConnectorListener edgeConnectorListener)
         : base(graphView, relayNodeData, edgeConnectorListener)
        {
            this.portType = type;

            OnCreate();
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void DrawNode()
        {
            
        }

        protected override void DrawPorts()
        {
            HNGraphPort inputPortData = RelayNodeData.InputPort;
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
            if(inputPortView.orientation == Orientation.Vertical)
            {
                TopPortContainer.Add(inputPortView);
            }
            else
            {
                inputContainer.Add(inputPortView);
            }

            HNGraphPort outputPortData = RelayNodeData.OutputPort;
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
            if(outputPortView.orientation == Orientation.Vertical)
            {
                BottomPortContainer.Add(outputPortView);
            }
            else
            {
                outputContainer.Add(outputPortView);
            }
        }
    }
}
