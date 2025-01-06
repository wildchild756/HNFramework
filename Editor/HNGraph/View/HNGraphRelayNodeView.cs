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

        public HNGraphRelayNodePortView InputPortView => inputPortView;

        public HNGraphRelayNodePortView OutputPortView => outputPortView;


        private HNGraphRelayNodePortView inputPortView;

        private HNGraphRelayNodePortView outputPortView;
    
    
        public HNGraphRelayNodeView(HNGraphView graphView, HNGraphRelayNode relayNodeData, HNGraphEdgeConnectorListener edgeConnectorListener)
         : base(graphView, relayNodeData, edgeConnectorListener)
        {
            RelayNodeData.EditorData = graphView.GraphEditorData;
            
            var nodeBorder = this.Q("node-border");
            nodeBorder.Remove(nodeBorder.Q("title"));

            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void AddPortView(HNGraphBasePortView portView)
        {
            if(portView is not HNGraphRelayNodePortView)
                return;

            if(portView.direction == Direction.Input)
            {
                inputContainer.Add(portView);
                inputPortView = portView as HNGraphRelayNodePortView;
                baseNodeData.AddInputPort(portView.PortData);
            }
            else
            {
                outputContainer.Add(portView);
                outputPortView = portView as HNGraphRelayNodePortView;
                baseNodeData.AddOutputPort(portView.PortData);
            }
        }

        protected override void DrawNode()
        {
            
        }

        protected override void DrawPorts()
        {
            HNGraphRelayNodePort inputPortData = new HNGraphRelayNodePort(
                RelayNodeData, 
                GraphView.GraphEditorData, 
                "", 
                "", 
                HNGraphRelayNodePort.Direction.Input, 
                HNGraphRelayNodePort.Capacity.Single
            );
            HNGraphRelayNodePortView inputPortView = new HNGraphRelayNodePortView(
                GraphView,
                inputPortData,
                this,
                "",
                Orientation.Horizontal,
                Direction.Input,
                Port.Capacity.Single,
                EdgeConnectorListener,
                InputPortView
            );
            AddPortView(inputPortView);

            HNGraphRelayNodePort outputPortData = new HNGraphRelayNodePort(
                RelayNodeData, 
                GraphView.GraphEditorData, 
                "", 
                "", 
                HNGraphRelayNodePort.Direction.Output, 
                HNGraphRelayNodePort.Capacity.Single
            );
            HNGraphRelayNodePortView outputPortView = new HNGraphRelayNodePortView(
                GraphView,
                outputPortData,
                this,
                "",
                Orientation.Horizontal,
                Direction.Output,
                Port.Capacity.Single,
                EdgeConnectorListener,
                OutputPortView
            );
            AddPortView(outputPortView);
        }

        public void CreateRelayNodeOnEdge(HNGraphEdgeView originEdgeView, HNGraphRelayNodeView relayNodeView)
        {
            if(originEdgeView == null || relayNodeView == null)
                return;
            
            HNGraphBasePortView outputPortView = originEdgeView.OutputPortView;
            HNGraphEdge outputEdge = new HNGraphEdge(graphView.GraphEditorData, OutputPortView.PortData, relayNodeView.InputPortView.PortData);
            outputEdge.Initialize();
            HNGraphEdgeView outputEdgeView = new HNGraphEdgeView(graphView);
            outputEdgeView.Initialize(outputEdge, outputPortView, relayNodeView.InputPortView);

            HNGraphBasePortView inputPortView = originEdgeView.InputPortView;
            HNGraphEdge inputEdge = new HNGraphEdge(graphView.GraphEditorData, relayNodeView.InputPortView.PortData, inputPortView.PortData);
            inputEdge.Initialize();
            HNGraphEdgeView inputEdgeView = new HNGraphEdgeView(graphView);
            inputEdgeView.Initialize(inputEdge, relayNodeView.OutputPortView, inputPortView);

            originEdgeView.DisconnectAll();
            graphView.RemoveElement(originEdgeView);

            graphView.AddElement(outputEdgeView);
            graphView.AddElement(inputEdgeView);
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
