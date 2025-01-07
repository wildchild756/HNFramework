using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using HN.Serialize;

namespace HN.Graph.Editor
{
    public partial class HNGraphView : GraphView
    {
        public void AddEdge(HNGraphEdgeView edgeView)
        {
            if (edgeView.output == null && edgeView.input == null)
                return;

            GraphEditorData.Owner.RecordObject("Add Edge");
            HNGraphBasePortView outputBasePortView = edgeView.OutputPortView;
            HNGraphPortView refOutputPortView = outputBasePortView.RefPortView;
            
            HNGraphBasePortView inputBasePortView = edgeView.InputPortView;
            HNGraphPortView refInputPortView = inputBasePortView.RefPortView;

            HNGraphEdge edgeData = edgeView.EdgeData;
            GraphEditorData.AddEdge(edgeData);

            UpdateOutputRefPortView(edgeView.OutputPortView, refOutputPortView);
            UpdateInputRefPortView(edgeView.InputPortView, refInputPortView);
            
            AddEdgeView(edgeData, ref edgeView);

            OnEdgeAdded(edgeView);
            graphViewElementsCreated(new HNGraphViewCreateElements(edgeView));
        }

        private void OnEdgeAdded(HNGraphEdgeView connectionView)
        {
            connectionView?.InputPortView?.OwnerNodeView?.BaseNodeData.OnConnectionAdded(connectionView.InputPortView.PortData.Guid);
            connectionView?.OutputPortView?.OwnerNodeView?.BaseNodeData.OnConnectionAdded(connectionView.OutputPortView.PortData.Guid);
        }

        public void RemoveEdge(HNGraphEdgeView edgeView)
        {
            if(edgeView == null)
                return;    

            UpdateOutputRefPortView(edgeView.OutputPortView, null);
            UpdateInputRefPortView(edgeView.InputPortView, null);
            edgeView.DisconnectOutput();
            edgeView.DisconnectInput();
            GraphEditorData.RemoveEdge(edgeView.EdgeData);
            RemoveElement(edgeView);
            edgeViews.Remove(edgeView);
        }


        private void DrawEdges()
        {
            foreach(var edgeData in GraphEditorData.Edges.Values)
            {
                HNGraphEdgeView edgeView = null;
                AddEdgeView(edgeData, ref edgeView);
            }

        }

        private void AddEdgeView(HNGraphEdge edgeData, ref HNGraphEdgeView edgeView)
        {
            if(edgeView == null)
            {
                edgeView = new HNGraphEdgeView(this);
                
                HNGraphNodeView outputPortNodeView = GetNodeViewFromGuid(edgeData.OutputPort.OwnerNode.Guid);
                string outputPortGuid = edgeData.OutputPort.Guid;
                foreach(var outputPortView in outputPortNodeView.OutputPortViews)
                {
                    if(outputPortView.PortData.Guid == outputPortGuid)
                    {
                        edgeView.output = outputPortView;
                        break;
                    }
                }

                HNGraphNodeView inputPortNodeView = GetNodeViewFromGuid(edgeData.InputPort.OwnerNode.Guid);
                string inputPortGuid = edgeData.InputPort.Guid;
                foreach(var inputPortView in inputPortNodeView.InputPortViews)
                {
                    if(inputPortView.PortData.Guid == inputPortGuid)
                    {
                        edgeView.input = inputPortView;
                        break;
                    }
                }
            }

            edgeView.Initialize(edgeData, edgeView.OutputPortView, edgeView.InputPortView);
            edgeViews.Add(edgeView);
            AddElement(edgeView);
        }

        private void UpdateOutputRefPortView(HNGraphBasePortView outputBasePortView, HNGraphPortView newOutputPortView)
        {
            if(outputBasePortView == null || outputBasePortView.OwnerNodeView == null)
                return;
            
            if(outputBasePortView.OwnerNodeView is HNGraphNodeView)
            {
                outputBasePortView.UpdateRefPortView(newOutputPortView);
            }
            else if(outputBasePortView.OwnerNodeView is HNGraphRelayNodeView)
            {
                HNGraphRelayNodeView relayNodeView = outputBasePortView.OwnerNodeView as HNGraphRelayNodeView;
                if(relayNodeView.InputPortView.EdgeViews.Count == 0)
                    return;
                
                foreach(var edgeView in relayNodeView.InputPortView.EdgeViews)
                {
                    HNGraphBasePortView anotherPortView = edgeView.OutputPortView;
                    UpdateOutputRefPortView(anotherPortView, newOutputPortView);
                }
            }
        }

        private void UpdateInputRefPortView(HNGraphBasePortView inputBasePortView, HNGraphPortView newInputPortView)
        {
            if(inputBasePortView == null || inputBasePortView.OwnerNodeView == null)
                return;

            if(inputBasePortView.OwnerNodeView is HNGraphNodeView)
            {
                inputBasePortView.UpdateRefPortView(newInputPortView);
            }
            else if(inputBasePortView.OwnerNodeView is HNGraphRelayNodeView)
            {
                HNGraphRelayNodeView relayNodeView = inputBasePortView.OwnerNodeView as HNGraphRelayNodeView;
                if(relayNodeView.OutputPortView.EdgeViews.Count == 0)
                    return;
                
                foreach(var edgeView in relayNodeView.OutputPortView.EdgeViews)
                {
                    HNGraphBasePortView anotherPortView = edgeView.InputPortView;
                    UpdateInputRefPortView(anotherPortView, newInputPortView);
                }
            }
        }

        

        private HNGraphEdgeView GetEdgeViewFromGuid(string guid)
        {
            foreach(var edgeView in edgeViews)
            {
                if(guid == edgeView.EdgeData.Guid)
                    return edgeView;
            }
            return null;
        }

    }
}
