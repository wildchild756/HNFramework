using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public abstract class HNGraphBaseNodeView : Node
    {
        public HNGraphBaseNode BaseNodeData => baseNodeData;
        
        public HNGraphEdgeConnectorListener EdgeConnectorListener => edgeConnectorListener;

        public IReadOnlyList<HNGraphPortView> InputPortViews => inputPortViews;

        public IReadOnlyList<HNGraphPortView> OutputPortViews => outputPortViews;

        public VisualElement TopPortContainer => topPortContainer;

        public VisualElement BottomPortContainer => bottomPortContainer;

        public HNGraphView GraphView => graphView;


        private HNGraphBaseNode baseNodeData;

        private HNGraphEdgeConnectorListener edgeConnectorListener;

        private List<HNGraphPortView> inputPortViews;

        private List<HNGraphPortView> outputPortViews;

        private VisualElement topPortContainer;

        private VisualElement bottomPortContainer;

        private HNGraphView graphView;


        public HNGraphBaseNodeView(HNGraphView graphView, HNGraphBaseNode nodeData, HNGraphEdgeConnectorListener edgeConnectorListener)
        {
            this.graphView = graphView;
            this.edgeConnectorListener = edgeConnectorListener;
            this.baseNodeData = nodeData;
            inputPortViews = new List<HNGraphPortView>();
            outputPortViews = new List<HNGraphPortView>();

            topPortContainer = new VisualElement();
            topPortContainer.name = "TopPortContainer";
            this.Insert(0, topPortContainer);
            bottomPortContainer = new VisualElement();
            bottomPortContainer.name = "BottomPortContainer";
            this.Add(bottomPortContainer);
        }

        public virtual void Initialize()
        {
            DrawNode();
            DrawPorts();
            SetPosition(baseNodeData.GetLayout());
        }

        public void AddPortView(HNGraphPortView portView)
        {
            if(portView.direction == Direction.Input)
            {
                if(portView.orientation == Orientation.Vertical)
                {
                    TopPortContainer.Add(portView);
                }
                else
                {
                    inputContainer.Add(portView);
                }
                inputPortViews.Add(portView);

                baseNodeData.AddInputPort(portView.PortData);
            }
            else
            {
                if(portView.orientation == Orientation.Vertical)
                {
                    BottomPortContainer.Add(portView);
                }
                else
                {
                    outputContainer.Add(portView);
                }
                outputPortViews.Add(portView);

                baseNodeData.AddOutputPort(portView.PortData);
            }
        }

        public void RemovePortView(HNGraphPortView portView)
        {
            if(inputContainer.Contains(portView))
            {
                inputContainer.Remove(portView);
                baseNodeData.RemoveInputPort(portView.PortData.Guid);
            }
            if(topPortContainer.Contains(portView))
            {
                topPortContainer.Remove(portView);
                baseNodeData.RemoveInputPort(portView.PortData.Guid);
            }
            if(outputContainer.Contains(portView))
            {
                outputContainer.Remove(portView);
                baseNodeData.RemoveOutputPort(portView.PortData.Guid);
            }
            if(bottomPortContainer.Contains(portView))
            {
                bottomPortContainer.Remove(portView);
                baseNodeData.RemoveOutputPort(portView.PortData.Guid);
            }
        }

        protected abstract void DrawNode();

        protected abstract void DrawPorts();

        public void SavePosition()
        {
            baseNodeData.SetLayout(GetPosition());
        }
    }
}
