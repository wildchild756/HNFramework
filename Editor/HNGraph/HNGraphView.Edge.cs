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
            // HNGraphBasePortView outputBasePortView = edgeView.OutputPortView;
            // HNGraphPortView refOutputPortView = outputBasePortView.RefPortView;
            
            // HNGraphBasePortView inputBasePortView = edgeView.InputPortView;
            // HNGraphPortView refInputPortView = inputBasePortView.RefPortView;

            HNGraphEdge edgeData = edgeView.EdgeData;
            GraphEditorData.AddEdge(edgeData);

            // UpdateOutputRefPortView(edgeView.OutputPortView, refOutputPortView);
            // UpdateInputRefPortView(edgeView.InputPortView, refInputPortView);
            UpdateRefPortView(edgeView);
            
            AddEdgeView(edgeData, ref edgeView);

            graphViewElementsCreated(new HNGraphViewCreateElements(edgeView));
        }

        public void RemoveEdge(HNGraphEdgeView edgeView)
        {
            if(edgeView == null)
                return;    
            
            // UpdateOutputRefPortView(edgeView.OutputPortView, null);
            // UpdateInputRefPortView(edgeView.InputPortView, null);
            UpdateDownstreamRelayNodeRefPort(edgeView, null);
            edgeView.DisconnectOutput();
            edgeView.DisconnectInput();
            GraphEditorData.RemoveEdge(edgeView.EdgeData);
            RemoveGraphElement(edgeView);
        }


        private void DrawEdges()
        {
            foreach(var edgeData in GraphEditorData.Edges.Values)
            {
                HNGraphEdgeView edgeView = null;
                AddEdgeView(edgeData, ref edgeView);
            }

        }

        public void AddEdgeView(HNGraphEdge edgeData, ref HNGraphEdgeView edgeView)
        {
            if(edgeView == null)
            {
                edgeView = new HNGraphEdgeView(this);
                
                HNGraphBaseNodeView outputPortNodeView = GetNodeViewFromGuid(edgeData.OutputPort.OwnerNode.Guid);
                if(outputPortNodeView == null)
                    outputPortNodeView = GetRelayNodeViewFromGuid(edgeData.OutputPort.OwnerNode.Guid);
                string outputPortGuid = edgeData.OutputPort.Guid;
                HNGraphBasePortView outputPortView = null;
                foreach(var o in outputPortNodeView.OutputPortViews)
                    if(o.PortData.Guid == outputPortGuid)
                    {
                        outputPortView = o;
                        break;
                    }

                HNGraphBaseNodeView inputPortNodeView = GetNodeViewFromGuid(edgeData.InputPort.OwnerNode.Guid);
                if(inputPortNodeView == null)
                    inputPortNodeView = GetRelayNodeViewFromGuid(edgeData.InputPort.OwnerNode.Guid);
                string inputPortGuid = edgeData.InputPort.Guid;
                HNGraphBasePortView inputPortView = null;
                foreach(var i in inputPortNodeView.InputPortViews)
                {
                    if(i.PortData.Guid == inputPortGuid)
                    {
                        inputPortView = i;
                        break;
                    }
                }

                edgeView.Initialize(edgeData, outputPortView, inputPortView);
            }
            AddGraphElement(edgeView);
        }

        private void UpdateRefPortView(HNGraphEdgeView edgeView)
        {
            HNGraphPortView refPortView = FindUpstreamRefPortView(edgeView);
            UpdateUpstreamRelayNodeRefPort(edgeView, refPortView);
            UpdateDownstreamRelayNodeRefPort(edgeView, refPortView);
        }

        private HNGraphPortView FindUpstreamRefPortView(HNGraphEdgeView edgeView)
        {
            HNGraphBasePortView edgeOutputPortView = edgeView.OutputPortView;
            HNGraphBaseNodeView edgeOutputPortOwnerNodeView = edgeOutputPortView.OwnerNodeView;
            if(edgeOutputPortOwnerNodeView is HNGraphRelayNodeView)
            {
                HNGraphRelayNodeView upstreamRelayNodeView = edgeOutputPortOwnerNodeView as HNGraphRelayNodeView;
                var upstreamRelayNodeViewInputEdgeView = upstreamRelayNodeView.InputPortView.EdgeViews;
                if(upstreamRelayNodeViewInputEdgeView.Count > 0)
                {
                    HNGraphEdgeView nextEdgeView = upstreamRelayNodeViewInputEdgeView[0];
                    return FindUpstreamRefPortView(nextEdgeView);
                }
            }
            else if(edgeOutputPortOwnerNodeView is HNGraphNodeView)
            {
                return edgeOutputPortView as HNGraphPortView;
            }

            return null;
        }

        private void UpdateUpstreamRelayNodeRefPort(HNGraphEdgeView edgeView, HNGraphPortView newRefPortView)
        {
            HNGraphBasePortView edgeOutputPortView = edgeView.OutputPortView;
            HNGraphBaseNodeView edgeOutputPortOwnerNodeView = edgeOutputPortView.OwnerNodeView;
            if(edgeOutputPortOwnerNodeView is HNGraphRelayNodeView)
            {
                HNGraphRelayNodeView upstreamRelayNodeView = edgeOutputPortOwnerNodeView as HNGraphRelayNodeView;
                upstreamRelayNodeView.RelayNodeData.SetRefPort(newRefPortView.PortData);
                var upstreamRelayNodeViewInputEdgeView = upstreamRelayNodeView.InputPortView.EdgeViews;
                foreach(var inputEdgeView in upstreamRelayNodeViewInputEdgeView)
                {
                    UpdateUpstreamRelayNodeRefPort(inputEdgeView, newRefPortView);
                }
            }

            return;
        }

        private void UpdateDownstreamRelayNodeRefPort(HNGraphEdgeView edgeView, HNGraphPortView newRefPortView)
        {
            HNGraphBasePortView edgeInputPortView = edgeView.InputPortView;
            HNGraphBaseNodeView edgeInputPortOwnerNodeView = edgeInputPortView.OwnerNodeView;
            if(edgeInputPortOwnerNodeView is HNGraphRelayNodeView)
            {
                HNGraphRelayNodeView upstreamRelayNodeView = edgeInputPortOwnerNodeView as HNGraphRelayNodeView;
                upstreamRelayNodeView.RelayNodeData.SetRefPort(newRefPortView.PortData);
                var upstreamRelayNodeViewOutputEdgeView = upstreamRelayNodeView.OutputPortView.EdgeViews;
                foreach(var outputEdgeView in upstreamRelayNodeViewOutputEdgeView)
                {
                    UpdateUpstreamRelayNodeRefPort(outputEdgeView, newRefPortView);
                }
            }

            return;
        }


        // private void UpdateOutputRefPortView(HNGraphBasePortView outputBasePortView, HNGraphPortView newOutputPortView)
        // {
        //     if(outputBasePortView == null || outputBasePortView.OwnerNodeView == null)
        //         return;
            
        //     if(outputBasePortView.OwnerNodeView is HNGraphNodeView)
        //     {
        //         outputBasePortView.UpdateRefPortView(newOutputPortView);
        //     }
        //     else if(outputBasePortView.OwnerNodeView is HNGraphRelayNodeView)
        //     {
        //         HNGraphRelayNodeView relayNodeView = outputBasePortView.OwnerNodeView as HNGraphRelayNodeView;
        //         if(relayNodeView.InputPortView.EdgeViews.Count == 0)
        //             return;
                
        //         foreach(var edgeView in relayNodeView.InputPortView.EdgeViews)
        //         {
        //             HNGraphBasePortView anotherPortView = edgeView.OutputPortView;
        //             UpdateOutputRefPortView(anotherPortView, newOutputPortView);
        //         }
        //     }
        // }

        // private void UpdateInputRefPortView(HNGraphBasePortView inputBasePortView, HNGraphPortView newInputPortView)
        // {
        //     if(inputBasePortView == null || inputBasePortView.OwnerNodeView == null)
        //         return;

        //     if(inputBasePortView.OwnerNodeView is HNGraphNodeView)
        //     {
        //         inputBasePortView.UpdateRefPortView(newInputPortView);
        //     }
        //     else if(inputBasePortView.OwnerNodeView is HNGraphRelayNodeView)
        //     {
        //         HNGraphRelayNodeView relayNodeView = inputBasePortView.OwnerNodeView as HNGraphRelayNodeView;
        //         if(relayNodeView.OutputPortView.EdgeViews.Count == 0)
        //             return;
                
        //         foreach(var edgeView in relayNodeView.OutputPortView.EdgeViews)
        //         {
        //             HNGraphBasePortView anotherPortView = edgeView.InputPortView;
        //             UpdateInputRefPortView(anotherPortView, newInputPortView);
        //         }
        //     }
        // }

        

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
