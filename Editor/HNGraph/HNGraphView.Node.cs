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
        public void AddNode(string nodeDataTypeName, Vector2 graphMousePosition)
        {
            HNGraphNode nodeData = new HNGraphNode(nodeDataTypeName);
            nodeData.Initialize(graphMousePosition, GraphEditorData);
            GraphEditorData.Owner.RecordObject("Add Node");
            GraphEditorData.AddNode(nodeData);

            AddNodeView(nodeData, out HNGraphNodeView nodeView);

            graphViewElementsCreated(new HNGraphViewCreateElements(nodeView));
        }

        private void DrawNodes()
        {
            foreach(var nodeData in GraphEditorData.Nodes.Values)
            {
                AddNodeView(nodeData, out HNGraphNodeView nodeView);
            }
        }

        private void AddNodeView(HNGraphNode nodeData, out HNGraphNodeView nodeView)
        {
            nodeView = new HNGraphNodeView(this, nodeData, edgeConnectorListener);
            nodeView.Initialize();
            AddGraphElement(nodeView);
        }

        private void RemoveNode(HNGraphNodeView nodeView)
        {
            foreach(var portView in nodeView.InputPortViews)
            {
                var edgeGuids = portView.PortData.EdgeGuids;
                for(int i = 0; i < edgeGuids.Count; i++)
                {
                    HNGraphEdgeView edgeView = GetEdgeViewFromGuid(edgeGuids[i]);
                    RemoveEdge(edgeView);
                }
            }
            GraphEditorData.RemoveNode(nodeView.NodeData);
            RemoveGraphElement(nodeView);
        }

        private HNGraphNodeView GetNodeViewFromGuid(string guid)
        {
            foreach(var nodeView in nodeViews)
            {
                if(guid == nodeView.NodeData.Guid)
                    return nodeView;
            }
            return null;
        }

    }
}
