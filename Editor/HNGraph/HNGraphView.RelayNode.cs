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
        public void AddRelayNode(HNGraphEdgeView edgeView, Vector2 mousePos)
        {
            GraphEditorData.Owner.RecordObject("Add Relay Node");
            HNGraphEdge edgeData = edgeView.EdgeData;
            HNGraphRelayNode relayNodeData = new HNGraphRelayNode(edgeData);
            relayNodeData.Initialize(mousePos);
            GraphEditorData.AddRelayNode(relayNodeData);

            AddRelayNodeView(relayNodeData, out HNGraphRelayNodeView relayNodeView);
            relayNodeView.CreateRelayNodeOnEdge(edgeView, relayNodeView);

            graphViewElementsCreated(new HNGraphViewCreateElements(relayNodeView));
        }

        public void RemoveRelayNode(HNGraphRelayNodeView relayNodeView)
        {
            if(!ContainsElement(relayNodeView))
                return;

            relayNodeView.RemovePortView(GraphEditorData, relayNodeView.InputPortView);
            relayNodeView.RemovePortView(GraphEditorData, relayNodeView.OutputPortView);

            HNGraphBasePortView outputPortView = null, inputPortView = null;

            HNGraphBasePortView relayNodeInputPortView = relayNodeView.InputPortView;
            if(relayNodeInputPortView.EdgeViews.Count > 0)
            {
                HNGraphEdgeView inputEdgeView = relayNodeInputPortView.EdgeViews[0];
                outputPortView = inputEdgeView.GetAnotherPort(relayNodeInputPortView);
                RemoveEdge(inputEdgeView);
            }

            HNGraphBasePortView relayNodeOutputPortView = relayNodeView.OutputPortView;
            if(relayNodeOutputPortView.EdgeViews.Count > 0)
            {
                HNGraphEdgeView outputEdgeView = relayNodeOutputPortView.EdgeViews[0];
                inputPortView = outputEdgeView.GetAnotherPort(relayNodeOutputPortView);
                RemoveEdge(outputEdgeView);
            }

            RemoveGraphElement(relayNodeView);
            GraphEditorData.RemoveRelayNode(relayNodeView.RelayNodeData);

            if(outputPortView != null && inputPortView != null)
            {
                HNGraphEdge newEdge = new HNGraphEdge(outputPortView.PortData, inputPortView.PortData);
                newEdge.Initialize();
                HNGraphEdgeView newEdgeView = new HNGraphEdgeView(this);
                newEdgeView.Initialize(newEdge, outputPortView, inputPortView);
                AddEdge(newEdgeView);
            }

            EditorUtility.SetDirty(GraphEditorWindow);
        }

        private void DrawRelayNodes()
        {
            foreach(string relayNodeGuid in GraphEditorData.GetAllRelayNodeGuids())
            {
                HNGraphRelayNode relayNodeData = GraphEditorData.GetRelayNode(relayNodeGuid);
                AddRelayNodeView(relayNodeData, out HNGraphRelayNodeView relayNodeView);
            }
        }

        private void AddRelayNodeView(HNGraphRelayNode relayNodeData, out HNGraphRelayNodeView relayNodeView)
        {
            relayNodeView = new HNGraphRelayNodeView(this, relayNodeData, edgeConnectorListener);
            relayNodeView.Initialize(GraphEditorData);
            AddGraphElement(relayNodeView);
        }

        private HNGraphRelayNodeView GetRelayNodeViewFromGuid(string guid)
        {
            foreach(var relayNodeView in relayNodeViews)
            {
                if(guid == relayNodeView.RelayNodeData.Guid)
                    return relayNodeView;
            }
            return null;
        }
    

    }
}
