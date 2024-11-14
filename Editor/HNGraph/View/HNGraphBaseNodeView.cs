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
        private HNGraphBaseNode baseNodeData;

        public HNGraphEdgeConnectorListener EdgeConnectorListener => edgeConnectorListener;
        private HNGraphEdgeConnectorListener edgeConnectorListener;

        public List<HNGraphPortView> InputPortViews => inputPortViews;
        private List<HNGraphPortView> inputPortViews;

        public List<HNGraphPortView> OutputPortViews => outputPortViews;
        private List<HNGraphPortView> outputPortViews;

        public VisualElement TopPortContainer => topPortContainer;
        private VisualElement topPortContainer;

        public VisualElement BottomPortContainer => bottomPortContainer;
        private VisualElement bottomPortContainer;

        public HNGraphView GraphView;
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

        public virtual void OnCreate()
        {
            DrawNode();
            DrawPorts();
        }

        protected abstract void DrawNode();

        protected abstract void DrawPorts();

        public void SavePosition()
        {
            BaseNodeData.SetLayout(GetPosition());
        }
    }
}
