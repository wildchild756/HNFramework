using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace HN.Graph.Editor
{
    public class HNGraphNodeView : HNGraphBaseNodeView
    {        
        public HNGraphNode NodeData => BaseNodeData as HNGraphNode;
        

        // private Type nodeDataType;


        public HNGraphNodeView(HNGraphView graphView, HNGraphNode nodeData, HNGraphEdgeConnectorListener edgeConnectorListener) 
        : base(graphView, nodeData, edgeConnectorListener)
        {

        }

        public override void Initialize(HNGraphData editorData)
        {
            base.Initialize(editorData);
        }

        protected override void DrawNode(HNGraphData editorData)
        {
            Type nodeDataType = NodeData.GetNodeDataType(editorData);
            HNGraphNodeInfo info = nodeDataType.GetCustomAttribute<HNGraphNodeInfo>();
            title = info.NodeTitle;
            name = nodeDataType.Name;

            // string[] depths = info.MenuItem.Split('/');
            // foreach (string depth in depths)
            // {
            //     AddToClassList(depth.ToLower().Replace(' ', '-'));
            // }
        }

        protected override void DrawPorts(HNGraphData editorData)
        {
            Type nodeDataType = NodeData.GetNodeDataType(editorData);
            PropertyInfo[] propertiesInfo = nodeDataType.GetProperties();
            foreach (var propertyInfo in propertiesInfo)
            {
                HNGraphPortInfo slotInfo = propertyInfo.GetCustomAttribute<HNGraphPortInfo>();
                if (slotInfo != null)
                {
                    HNGraphBasePort port = null;

                    foreach(string inputPortGuid in NodeData.InputPortGuids)
                    {
                        var inputPort = graphView.GraphEditorData.GetNodePort(inputPortGuid);
                        if(inputPort.IsMatchWithAttribute(propertyInfo.PropertyType, slotInfo))
                        {
                            port = inputPort;
                        }
                    }

                    foreach(string outputPortGuid in NodeData.OutputPortGuids)
                    {
                        var outputPort = graphView.GraphEditorData.GetNodePort(outputPortGuid);
                        if(outputPort.IsMatchWithAttribute(propertyInfo.PropertyType, slotInfo))
                        {
                            port = outputPort;
                        }
                    }

                    if(port == null)
                    {
                        port = new HNGraphNodePort(
                            BaseNodeData.Guid,
                            propertyInfo.PropertyType.FullName,
                            slotInfo.PortName, 
                            propertyInfo.Name,
                            slotInfo.PortDirection == HNGraphPortInfo.Direction.Input ? HNGraphBasePort.Direction.Input : HNGraphBasePort.Direction.Output, 
                            slotInfo.PortCapacity == HNGraphPortInfo.Capacity.Single ? HNGraphBasePort.Capacity.Single : HNGraphBasePort.Capacity.Multi
                            );
                        
                        // if(port.PortDirection == HNGraphBasePort.Direction.Input)
                        //     BaseNodeData.AddInputPort(port);
                        // else if(port.PortDirection == HNGraphBasePort.Direction.Output)
                        //     BaseNodeData.AddOutputPort(port);
                    }

                    CreatePortView(editorData, port, slotInfo);
                }
            }
        }

        public override void AddPortView(HNGraphData editorData, HNGraphBasePortView portView)
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

                baseNodeData.AddInputPort(editorData, portView.PortData);
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

                baseNodeData.AddOutputPort(editorData, portView.PortData);
            }
        }

        public override void RemovePortView(HNGraphData editorData, HNGraphBasePortView portView)
        {
            if(inputContainer.Contains(portView))
            {
                inputContainer.Remove(portView);
                NodeData.RemoveInputPort(editorData, portView.PortData);
            }
            if(topPortContainer.Contains(portView))
            {
                topPortContainer.Remove(portView);
                NodeData.RemoveInputPort(editorData, portView.PortData);
            }
            if(outputContainer.Contains(portView))
            {
                outputContainer.Remove(portView);
                NodeData.RemoveOutputPort(editorData, portView.PortData);
            }
            if(bottomPortContainer.Contains(portView))
            {
                bottomPortContainer.Remove(portView);
                NodeData.RemoveOutputPort(editorData, portView.PortData);
            }
        }

        private void CreatePortView(HNGraphData editorData, HNGraphBasePort port, HNGraphPortInfo slotInfo)
        {
            HNGraphPortView portView = new HNGraphPortView(
                GraphView,
                port,
                this,
                slotInfo.PortName, 
                slotInfo.orientation == HNGraphPortInfo.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical,
                slotInfo.PortDirection == HNGraphPortInfo.Direction.Input ? Direction.Input : Direction.Output, 
                slotInfo.PortCapacity == HNGraphPortInfo.Capacity.Single ? Port.Capacity.Single : Port.Capacity.Multi,
                EdgeConnectorListener
                );
            AddPortView(editorData, portView);
        }

    }
}
