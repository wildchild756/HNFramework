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

        public HNGraphRelayNodePortView InputPortView
        {
            get { return (HNGraphRelayNodePortView)(inputPortViews.Count == 0 ? null : inputPortViews[0]); }
            set 
            { 
                if(inputPortViews.Count == 0)
                    inputPortViews.Add(value);
                else
                    inputPortViews[0] = value; 
            }
        }

        public HNGraphRelayNodePortView OutputPortView
        {
            get { return (HNGraphRelayNodePortView)(outputPortViews.Count == 0 ? null : outputPortViews[0]); }
            set 
            { 
                if(outputPortViews.Count == 0)
                    outputPortViews.Add(value);
                else
                    outputPortViews[0] = value; 
            }
        }


        // private HNGraphRelayNodePortView inputPortView;
        // private HNGraphRelayNodePortView outputPortView;
    
    
        public HNGraphRelayNodeView(HNGraphView graphView, HNGraphRelayNode relayNodeData, HNGraphEdgeConnectorListener edgeConnectorListener)
         : base(graphView, relayNodeData, edgeConnectorListener)
        {            
            var nodeBorder = this.Q("node-border");
            nodeBorder.Remove(nodeBorder.Q("title"));

            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void Initialize(HNGraphData editorData)
        {
            base.Initialize(editorData);
        }

        public override void AddPortView(HNGraphData editorData, HNGraphBasePortView portView)
        {
            if(portView is not HNGraphRelayNodePortView)
                return;

            if(portView.direction == Direction.Input)
            {
                inputContainer.Add(portView);
                InputPortView = portView as HNGraphRelayNodePortView;
                baseNodeData.AddInputPort(editorData, portView.PortData);
            }
            else
            {
                outputContainer.Add(portView);
                OutputPortView = portView as HNGraphRelayNodePortView;
                baseNodeData.AddOutputPort(editorData, portView.PortData);
            }
        }

        public override void RemovePortView(HNGraphData editorData, HNGraphBasePortView portView)
        {
            if(inputContainer.Contains(portView))
            {
                inputContainer.Remove(portView);
                RelayNodeData.RemoveInputPort(editorData, portView.PortData);
            }
            if(outputContainer.Contains(portView))
            {
                outputContainer.Remove(portView);
                RelayNodeData.RemoveOutputPort(editorData, portView.PortData);
            }
        }

        protected override void DrawNode(HNGraphData editorData)
        {
            
        }

        protected override void DrawPorts(HNGraphData editorData)
        {
            HNGraphRelayNodePort inputPortData = null;
            inputPortData = editorData.GetRelayNodePort(RelayNodeData.InputPortGuid);
            if(inputPortData == null)
                inputPortData = new HNGraphRelayNodePort(
                    RelayNodeData.Guid, 
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
                EdgeConnectorListener
            );
            AddPortView(editorData, inputPortView);

            HNGraphRelayNodePort outputPortData = null;
            outputPortData = editorData.GetRelayNodePort(RelayNodeData.OutputPortGuid);
            if(outputPortData == null)
                outputPortData = new HNGraphRelayNodePort(
                    RelayNodeData.Guid, 
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
                EdgeConnectorListener
            );
            AddPortView(editorData, outputPortView);
        }

        public void CreateRelayNodeOnEdge(HNGraphEdgeView originEdgeView, HNGraphRelayNodeView relayNodeView)
        {
            if(originEdgeView == null || relayNodeView == null)
                return;
            
            HNGraphBasePortView outputPortView = originEdgeView.OutputPortView;
            HNGraphEdge outputEdge = new HNGraphEdge(outputPortView.PortData, relayNodeView.InputPortView.PortData);
            outputEdge.Initialize();
            HNGraphEdgeView outputEdgeView = new HNGraphEdgeView(graphView);
            outputEdgeView.Initialize(outputEdge, outputPortView, relayNodeView.InputPortView);

            HNGraphBasePortView inputPortView = originEdgeView.InputPortView;
            HNGraphEdge inputEdge = new HNGraphEdge(relayNodeView.OutputPortView.PortData, inputPortView.PortData);
            inputEdge.Initialize();
            HNGraphEdgeView inputEdgeView = new HNGraphEdgeView(graphView);
            inputEdgeView.Initialize(inputEdge, relayNodeView.OutputPortView, inputPortView);

            graphView.RemoveEdge(originEdgeView);

            graphView.AddEdge(outputEdgeView);
            graphView.AddEdge(inputEdgeView);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if(e.altKey)
            {
                GraphView.RemoveRelayNode(this);
            }
        }
    }
}
