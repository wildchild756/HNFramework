using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphPortView : Port
    {
        public string PortName
        {
            get { return portName; }
            set { portName = value; }
        }
        
        public HNGraphPort PortData => portData;
        private HNGraphPort portData;

        public HNGraphNodeView OwnerNodeView => ownerNodeView;
        private HNGraphNodeView ownerNodeView;

        public List<HNGraphEdgeView> EdgeViews => edgeViews;
        private List<HNGraphEdgeView> edgeViews;


        public HNGraphPortView(
            HNGraphPort portData, 
            HNGraphNodeView nodeView, 
            string name, 
            Direction portDirection, 
            Capacity capacity, 
            IEdgeConnectorListener connectListener
            ): base(Orientation.Horizontal, portDirection, capacity, null)
        {
            PortName = name;
            this.portData = portData;
            this.ownerNodeView = nodeView;
            edgeViews = new List<HNGraphEdgeView>();

            var edgeConnector = new HNGraphEdgeConnector(connectListener);
            this.AddManipulator(edgeConnector);
        }

        public void ConnectToEdge(HNGraphEdgeView edgeView)
        {
            Connect(edgeView);
            if(!edgeViews.Contains(edgeView))
            {
                edgeViews.Add(edgeView);
            }

            portData.ConnectToEdge(edgeView.EdgeData);
        }

        public void DisconnectFromEdge(HNGraphEdgeView edgeView)
        {
            Disconnect(edgeView);
            if(edgeViews.Contains(edgeView))
            {
                edgeViews.Remove(edgeView);
            }

            portData.DisconnectFromEdge(edgeView.EdgeData);
        }

        public List<HNGraphPortView> FindConnectPorts()
        {
            List<HNGraphPortView> connectPorts = new List<HNGraphPortView>();
            foreach(var edgeView in edgeViews)
            {
                 connectPorts.Add(edgeView.FindAnotherPort(this));
            }

            return connectPorts;
        }

        public HNGraphPortView FindFirstConnectPort()
        {
            if(edgeViews.Count > 0)
            {
                 return edgeViews[0].FindAnotherPort(this);
            }
            
            return null;
        }

        public List<HNGraphNodeView> FindConnectNodes()
        {
            List<HNGraphNodeView> connectNodes = new List<HNGraphNodeView>();
            List<HNGraphPortView> connectPorts = FindConnectPorts();
            foreach(var port in connectPorts)
            {
                connectNodes.Add(port.OwnerNodeView);
            }

            return connectNodes;
        }

        public HNGraphNodeView FindFirstConnectNode()
        {
            HNGraphPortView connectPort = FindFirstConnectPort();
            if(connectPort != null)
            {
                return connectPort.OwnerNodeView;
            }

            return null;
        }

        public bool IsComptibleWith(HNGraphPortView portView)
        {
            return portView != null
                && portView.node != node
                && portView.direction != direction
                && (!portView.connected || (portView.connected && portView.capacity == Capacity.Multi));
        }
    }
}
