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
    public class HNGraphNodeView : Node
    {
        public HNGraphNode NodeData => nodeData;
        private HNGraphNode nodeData;

        public HNGraphEdgeConnectorListener EdgeConnectorListener => edgeConnectorListener;
        private HNGraphEdgeConnectorListener edgeConnectorListener;

        public List<HNGraphPortView> InputPortViews => inputPortViews;
        private List<HNGraphPortView> inputPortViews;

        public List<HNGraphPortView> OutputPortViews => outputPortViews;
        private List<HNGraphPortView> outputPortViews;

        private Type passType;


        public HNGraphNodeView(HNGraphNode nodeData, HNGraphEdgeConnectorListener edgeConnectorListener) : base()
        {
            this.edgeConnectorListener = edgeConnectorListener;
            this.nodeData = nodeData;
            inputPortViews = new List<HNGraphPortView>();
            outputPortViews = new List<HNGraphPortView>();
            passType = nodeData.GraphNodeClass.GetType();

            DrawNode();
            DrawPort();
        }

        private void DrawNode()
        {
            HNGraphNodeInfoAttribute info = passType.GetCustomAttribute<HNGraphNodeInfoAttribute>();
            title = info.NodeTitle;
            name = passType.Name;

            // string[] depths = info.MenuItem.Split('/');
            // foreach (string depth in depths)
            // {
            //     AddToClassList(depth.ToLower().Replace(' ', '-'));
            // }
        }

        private void DrawPort()
        {
            PropertyInfo[] propertiesInfo = passType.GetProperties();
            foreach (var propertyInfo in propertiesInfo)
            {
                HNGraphPortInfoAttribute slotInfo = propertyInfo.GetCustomAttribute<HNGraphPortInfoAttribute>();
                if (slotInfo != null)
                {
                    HNGraphPort port = null;
                    foreach(var nodePort in nodeData.InputPorts.Values)
                    {
                        if(nodePort.IsMatchWithAttribute(propertyInfo.PropertyType, slotInfo))
                        {
                            port = nodePort;
                        }
                    }
                    foreach(var nodePort in nodeData.OutputPorts.Values)
                    {
                        if(nodePort.IsMatchWithAttribute(propertyInfo.PropertyType, slotInfo))
                        {
                            port = nodePort;
                        }
                    }
                    if(port == null)
                    {
                        port = new HNGraphPort(
                            nodeData, 
                            propertyInfo.PropertyType.FullName, 
                            slotInfo.PortName, 
                            slotInfo.PortDirection == HNGraphPortInfoAttribute.Direction.Input ? HNGraphPort.Direction.Input : HNGraphPort.Direction.Output, 
                            slotInfo.PortCapacity == HNGraphPortInfoAttribute.Capacity.Single ? HNGraphPort.Capacity.Single : HNGraphPort.Capacity.Multi
                            );
                        nodeData.AddPort(port);
                    }

                    CreatePortView(port, slotInfo);
                }
            }
        }

        private void CreatePortView(HNGraphPort port, HNGraphPortInfoAttribute slotInfo)
        {
            HNGraphPortView portView = new HNGraphPortView(
                port,
                this,
                slotInfo.PortName, 
                slotInfo.PortDirection == HNGraphPortInfoAttribute.Direction.Input ? Direction.Input : Direction.Output, 
                slotInfo.PortCapacity == HNGraphPortInfoAttribute.Capacity.Single ? Port.Capacity.Single : Port.Capacity.Multi,
                edgeConnectorListener
                );
            if (slotInfo.PortDirection == HNGraphPortInfoAttribute.Direction.Input)
            {
                inputPortViews.Add(portView);
                inputContainer.Add(portView);
            }
            else
            {
                outputPortViews.Add(portView);
                outputContainer.Add(portView);
            }
        }

        public void SavePosition()
        {
            nodeData.SetPosition(GetPosition());
        }
    }
}
