using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphConnectionView
    {
        public HNGraphConnection ConnectionData
        {
            get { return connectionData; }
            set { connectionData = value; }
        }
        
        public HNGraphPortView OutputPortView
        {
            get { return edges.Count > 0 ? (HNGraphPortView)edges[0].output : null; }
            set { 
                    if(edges.Count > 0)
                        edges[0].output = value; 
                }
        }

        public HNGraphPortView InputPortView
        {
            get { return edges.Count > 0 ? (HNGraphPortView)edges[edges.Count - 1].input : null; }
            set { 
                    if(edges.Count > 0)
                        edges[edges.Count - 1].input = value;
                }
        }

        public HNGraphEdgeView OutputEdgeView => edges.Count > 0 ? edges[0] : null;

        public HNGraphEdgeView InputEdgeView => edges.Count > 0 ? edges[edges.Count - 1] : null;

        public IReadOnlyList<HNGraphEdgeView> Edges => edges;

        public IReadOnlyList<HNGraphRelayNodeView> RelayNodes => relayNodes;


        private HNGraphConnection connectionData;

        private List<HNGraphEdgeView> edges;

        private List<HNGraphRelayNodeView> relayNodes;

        private HNGraphView graphView;


        public HNGraphConnectionView(HNGraphView graphView, HNGraphConnection connectionData)
        {
            this.graphView = graphView;
            this.connectionData = connectionData;
            edges = new List<HNGraphEdgeView>();
            relayNodes = new List<HNGraphRelayNodeView>();
        }

        public void Initialize(HNGraphPortView outputPortView, HNGraphPortView inputPortView)
        {
            outputPortView.ConnectToConnectionOutput(this);
            inputPortView.ConnectToConnectionInput(this);
        }

        public HNGraphPortView GetAnotherPort(HNGraphPortView port)
        {
            if(OutputPortView == port)
            {
                return InputPortView;
            }
            else if(InputPortView == port)
            {
                return OutputPortView;
            }
            else
            {
                return null;
            }
        }

        public void CreateRelayNodeOnEdge(HNGraphEdgeView originEdgeView, HNGraphRelayNodeView relayNodeView)
        {
            HNGraphPortView outputPortView = originEdgeView.OutputPortView;
            HNGraphEdgeView outputEdgeView = new HNGraphEdgeView(graphView);
            outputEdgeView.Initialize(this, outputPortView, relayNodeView.InputPortView);
            edges.Add(outputEdgeView);
            graphView.AddElement(outputEdgeView);

            HNGraphPortView inputPortView = originEdgeView.InputPortView;
            HNGraphEdgeView inputEdgeView = new HNGraphEdgeView(graphView);
            inputEdgeView.Initialize(this, relayNodeView.OutputPortView, inputPortView);
            edges.Add(inputEdgeView);
            graphView.AddElement(inputEdgeView);

            HNGraphBaseNodeView nextNodeView = inputPortView.OwnerNodeView;
            int index = 0;
            for(; index < relayNodes.Count; index++)
            {
                if(nextNodeView == relayNodes[index])
                    break;
            }
            relayNodes.Insert(index, relayNodeView);
            connectionData.AddRelayNode(index, relayNodeView.RelayNodeData);

            originEdgeView.DisconnectAll();
            if(edges.Contains(originEdgeView))
            {
                edges.Remove(originEdgeView);
                graphView.RemoveElement(originEdgeView);
            }
            graphView.AddElement(relayNodeView);
        }

        public void RemoveAllEdgeViews()
        {
            for(int i = edges.Count - 1; i >= 0; i--)
            {
                graphView.RemoveElement(edges[i]);
                RemoveEdgeView(edges[i]);
            }
        }

        public void AddEdgeView(HNGraphEdgeView edgeView)
        {
            edges.Add(edgeView);
        }

        public void RemoveEdgeView(HNGraphEdgeView edgeView)
        {
            edgeView.DisconnectAll();
            edges.Remove(edgeView);
        }

        public void AddRelayNodeView(int index, HNGraphRelayNodeView relayNodeView)
        {
            relayNodes.Insert(index, relayNodeView);
        }

        public void RemoveRelayNodeView(HNGraphRelayNodeView relayNodeView)
        {
            relayNodes.Remove(relayNodeView);
        }

    }
}
