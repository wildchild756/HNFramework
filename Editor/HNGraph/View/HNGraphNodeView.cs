using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

namespace HN.Graph.Editor
{
    public class HNGraphNodeView : HNGraphBaseNodeView
    {
        public IReadOnlyList<HNGraphPortView> InputPortViews => inputPortViews;

        public IReadOnlyList<HNGraphPortView> OutputPortViews => outputPortViews;
        
        public HNGraphNode NodeData => BaseNodeData as HNGraphNode;


        protected List<HNGraphPortView> inputPortViews;

        protected List<HNGraphPortView> outputPortViews;
        

        private Type nodeDataType;


        public HNGraphNodeView(HNGraphView graphView, HNGraphNode nodeData, HNGraphEdgeConnectorListener edgeConnectorListener) 
        : base(graphView, nodeData, edgeConnectorListener)
        {
            NodeData.EditorData = graphView.GraphEditorData;
            inputPortViews = new List<HNGraphPortView>();
            outputPortViews = new List<HNGraphPortView>();
        }

        public override void Initialize()
        {
            nodeDataType = NodeData.NodeDataType;
            base.Initialize();
        }

        protected override void DrawNode()
        {
            HNGraphNodeInfo info = nodeDataType.GetCustomAttribute<HNGraphNodeInfo>();
            title = info.NodeTitle;
            name = nodeDataType.Name;

            // string[] depths = info.MenuItem.Split('/');
            // foreach (string depth in depths)
            // {
            //     AddToClassList(depth.ToLower().Replace(' ', '-'));
            // }
        }

        protected override void DrawPorts()
        {
            PropertyInfo[] propertiesInfo = nodeDataType.GetProperties();
            foreach (var propertyInfo in propertiesInfo)
            {
                HNGraphPortInfo slotInfo = propertyInfo.GetCustomAttribute<HNGraphPortInfo>();
                if (slotInfo != null)
                {
                    HNGraphBasePort port = null;

                    foreach(var inputPort in NodeData.InputPorts.Values)
                    {
                        if(inputPort.IsMatchWithAttribute(propertyInfo.PropertyType, slotInfo))
                        {
                            port = inputPort;
                        }
                    }

                    foreach(var outputPort in NodeData.OutputPorts.Values)
                    {
                        if(outputPort.IsMatchWithAttribute(propertyInfo.PropertyType, slotInfo))
                        {
                            port = outputPort;
                        }
                    }

                    if(port == null)
                    {
                        port = new HNGraphPort(
                            BaseNodeData,
                            GraphView.GraphEditorData,
                            propertyInfo.PropertyType.FullName, 
                            slotInfo.PortName, 
                            slotInfo.PortDirection == HNGraphPortInfo.Direction.Input ? HNGraphBasePort.Direction.Input : HNGraphBasePort.Direction.Output, 
                            slotInfo.PortCapacity == HNGraphPortInfo.Capacity.Single ? HNGraphBasePort.Capacity.Single : HNGraphBasePort.Capacity.Multi
                            );
                        
                        if(port.PortDirection == HNGraphBasePort.Direction.Input)
                            BaseNodeData.AddInputPort(port);
                        else if(port.PortDirection == HNGraphBasePort.Direction.Output)
                            BaseNodeData.AddOutputPort(port);
                    }

                    CreatePortView(port, slotInfo);
                }
            }
        }

        protected override void AddPortView(HNGraphBasePortView portView)
        {
            if(portView is not HNGraphPortView)
                return;

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
                inputPortViews.Add(portView as HNGraphPortView);

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
                outputPortViews.Add(portView as HNGraphPortView);

                baseNodeData.AddOutputPort(portView.PortData);
            }
        }

        public void RemovePortView(HNGraphBasePortView portView)
        {
            if(inputContainer.Contains(portView))
            {
                inputContainer.Remove(portView);
                NodeData.RemoveInputPort(portView.PortData.Guid);
            }
            if(topPortContainer.Contains(portView))
            {
                topPortContainer.Remove(portView);
                NodeData.RemoveInputPort(portView.PortData.Guid);
            }
            if(outputContainer.Contains(portView))
            {
                outputContainer.Remove(portView);
                NodeData.RemoveOutputPort(portView.PortData.Guid);
            }
            if(bottomPortContainer.Contains(portView))
            {
                bottomPortContainer.Remove(portView);
                NodeData.RemoveOutputPort(portView.PortData.Guid);
            }
        }

        private void CreatePortView(HNGraphBasePort port, HNGraphPortInfo slotInfo)
        {
            HNGraphPortView portView = new HNGraphPortView(
                GraphView,
                port,
                this,
                slotInfo.PortName, 
                slotInfo.orientation == HNGraphPortInfo.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical,
                slotInfo.PortDirection == HNGraphPortInfo.Direction.Input ? Direction.Input : Direction.Output, 
                slotInfo.PortCapacity == HNGraphPortInfo.Capacity.Single ? Port.Capacity.Single : Port.Capacity.Multi,
                EdgeConnectorListener,
                null
                );
            AddPortView(portView);
        }

    }
}
