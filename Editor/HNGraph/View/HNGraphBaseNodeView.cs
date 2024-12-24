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


        public VisualElement TopPortContainer => topPortContainer;

        public VisualElement BottomPortContainer => bottomPortContainer;

        public HNGraphView GraphView => graphView;


        protected HNGraphBaseNode baseNodeData;

        protected HNGraphEdgeConnectorListener edgeConnectorListener;

        protected VisualElement topPortContainer;

        protected VisualElement bottomPortContainer;

        protected HNGraphView graphView;


        public HNGraphBaseNodeView(HNGraphView graphView, HNGraphBaseNode nodeData, HNGraphEdgeConnectorListener edgeConnectorListener)
        {
            this.graphView = graphView;
            this.edgeConnectorListener = edgeConnectorListener;
            this.baseNodeData = nodeData;

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

        protected abstract void AddPortView(HNGraphBasePortView portView);

        protected abstract void DrawNode();

        protected abstract void DrawPorts();

        public void SavePosition()
        {
            baseNodeData.SetLayout(GetPosition());
        }
    }
}
