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

        public HNGraphBaseNodeView OwnerNodeView => ownerNodeView;

        public IReadOnlyList<HNGraphConnectionView> ConnectionViews => connectionViews;

        public IReadOnlyList<HNGraphEdgeView> EdgeViews => edgeViews;


        private HNGraphPort portData;

        private HNGraphBaseNodeView ownerNodeView;

        private List<HNGraphConnectionView> connectionViews;

        private List<HNGraphEdgeView> edgeViews;

        private HNGraphView graphView;


        public HNGraphPortView(
            HNGraphView graphView,
            HNGraphPort portData, 
            HNGraphBaseNodeView nodeView, 
            string name,
            Orientation orientation,
            Direction portDirection, 
            Capacity capacity, 
            IEdgeConnectorListener connectListener
            ): base(orientation, portDirection, capacity, null)
        {
            this.graphView = graphView;
            PortName = orientation == Orientation.Horizontal ? name : "";
            tooltip = orientation == Orientation.Horizontal ? "" : name;
            this.portData = portData;
            this.ownerNodeView = nodeView;
            connectionViews = new List<HNGraphConnectionView>();
            edgeViews = new List<HNGraphEdgeView>();
            
            var edgeConnector = new HNGraphEdgeConnector(graphView, connectListener);
            this.AddManipulator(edgeConnector);
        }

        public void ConnectToConnectionOutput(HNGraphConnectionView connectionView)
        {
            if(connectionViews.Contains(connectionView))
            {
                // Debug.LogWarning($"{connectionViews} already contains connection view {connectionView}.");
                return;
            }
            connectionViews.Add(connectionView);

            portData.AddConnection(connectionView.ConnectionData.Guid);
            if(connectionView.OutputEdgeView != null)
                ConnectToEdge(connectionView.OutputEdgeView);
        }

        public void ConnectToConnectionInput(HNGraphConnectionView connectionView)
        {
            if(connectionViews.Contains(connectionView))
            {
                // Debug.LogWarning($"{connectionViews} already contains connection view {connectionView}.");
                return;
            }
            connectionViews.Add(connectionView);

            portData.AddConnection(connectionView.ConnectionData.Guid);
            if(connectionView.InputEdgeView != null)
                ConnectToEdge(connectionView.InputEdgeView);
        }

        public void ConnectToEdge(HNGraphEdgeView edgeView)
        {
            Connect(edgeView);
            if(!edgeViews.Contains(edgeView))
            {
                edgeViews.Add(edgeView);
            }
        }

        public void DisconnectFromConnectionOutput(HNGraphConnectionView connectionView)
        {
            Disconnect(connectionView.OutputEdgeView);
            if(connectionViews.Contains(connectionView))
            {
                connectionViews.Remove(connectionView);
            }

            portData.RemoveConnection(connectionView.ConnectionData.Guid);
        }

        public void DisconnectFromConnectionInput(HNGraphConnectionView connectionView)
        {
            Disconnect(connectionView.InputEdgeView);
            if(connectionViews.Contains(connectionView))
            {
                connectionViews.Remove(connectionView);
            }

            portData.RemoveConnection(connectionView.ConnectionData.Guid);
        }

        public void DisconnectFromEdge(HNGraphEdgeView edgeView)
        {
            if(edgeViews.Contains(edgeView))
            {
                Disconnect(edgeView);
                edgeViews.Remove(edgeView);
                OwnerNodeView.RefreshPorts();
            }
        }

        public List<HNGraphPortView> GetConnectPorts()
        {
            List<HNGraphPortView> connectPorts = new List<HNGraphPortView>();
            foreach(var connectionView in connectionViews)
            {
                 connectPorts.Add(connectionView.GetAnotherPort(this));
            }

            return connectPorts;
        }

        public HNGraphPortView GetFirstConnectPort()
        {
            if(connectionViews.Count > 0)
            {
                 return connectionViews[0].GetAnotherPort(this);
            }
            
            return null;
        }

        public List<HNGraphBaseNodeView> GetConnectNodes()
        {
            List<HNGraphBaseNodeView> connectNodes = new List<HNGraphBaseNodeView>();
            List<HNGraphPortView> connectPorts = GetConnectPorts();
            foreach(var port in connectPorts)
            {
                connectNodes.Add(port.OwnerNodeView);
            }

            return connectNodes;
        }

        public HNGraphBaseNodeView GetFirstConnectNode()
        {
            HNGraphPortView connectPort = GetFirstConnectPort();
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
