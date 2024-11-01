using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace HN.Graph.Editor
{
    public class HNGraphView : GraphView
    {
        public HNGraphEditorWindow graphEditorWindow;

        private HNGraphEditorData graphEditorData;
        private List<HNGraphNodeView> nodeViews;
        private List<HNGraphEdgeView> edgeViews;
        private HNGraphEdgeConnectorListener edgeConnectorListener;

        public HNGraphView(HNGraphEditorWindow graphEditorWindow, HNGraphEditorData graphEditorData, HNGraphSearchWindowProvider searchWindowProvider)
        {
            this.graphEditorWindow = graphEditorWindow;
            this.graphEditorData = graphEditorData;

            nodeViews = new List<HNGraphNodeView>();
            edgeViews = new List<HNGraphEdgeView>();

            serializeGraphElements = SerializeGraphElementsImplementation;

            AddManipulator(new ContentDragger());
            AddManipulator(new SelectionDragger());
            AddManipulator(new RectangleSelector());
            //AddManipulator(new ClickSelector());

            GridBackground gridBackground = new GridBackground();
            gridBackground.name = "GridBackground";
            Insert(0, gridBackground);

            SetupZoom(0.05f, 8);

            searchWindowProvider.graph = this;
            nodeCreationRequest += context =>
            {
                Vector2 pos = context.screenMousePosition;
                var searchWindowContext = new SearchWindowContext(pos, 0f, 0f);
                SearchWindow.Open(searchWindowContext, searchWindowProvider);
                EditorUtility.SetDirty(graphEditorWindow);
            };

            edgeConnectorListener = new HNGraphEdgeConnectorListener();
            graphViewChanged = RenderGraphViewChanged;

            DrawNodes();
            DrawEdges();

            styleSheets.Add(Resources.Load<StyleSheet>("Styles/HNGraphView"));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            HNGraphPortView startPortView = (HNGraphPortView)startPort;
            foreach (var nodeView in nodeViews)
            {
                List<HNGraphPortView> nodeViewPortViews = new List<HNGraphPortView>();
                if (startPortView.direction == Direction.Output)
                {
                    nodeViewPortViews = nodeView.InputPortViews;
                }
                else if (startPortView.direction == Direction.Input)
                {
                    nodeViewPortViews = nodeView.OutputPortViews;
                }
                foreach (var portView in nodeViewPortViews)
                {
                    if (startPortView.IsComptibleWith(portView))
                    {
                        compatiblePorts.Add(portView);
                    }
                }
            }
            return compatiblePorts;
        }

        public void AddNode(HNGraphNode node)
        {
            Undo.RecordObject(graphEditorData, "Add Node");
            graphEditorData.Nodes.Add(node);

            AddNodeView(node);
        }

        public void AddEdge(HNGraphEdgeView edgeView)
        {
            if (edgeView.OutputPortView == null && edgeView.InputPortView == null)
            {
                return;
            }
            Undo.RecordObject(graphEditorData, "Add Edge");
            HNGraphEdge connection = new HNGraphEdge(edgeView.OutputPortView.PortData, edgeView.InputPortView.PortData);
            edgeView.EdgeData = connection;
            graphEditorData.Edges.Add(connection);

            AddEdgeView(edgeView.OutputPortView, edgeView.InputPortView, edgeView);
        }


        private void DrawNodes()
        {
            foreach (HNGraphNode node in graphEditorData.Nodes)
            {
                AddNodeView(node);
            }
        }

        private void AddNodeView(HNGraphNode node)
        {
            HNGraphNodeView nodeView = new HNGraphNodeView(node, edgeConnectorListener);
            nodeView.SetPosition(node.Position);
            nodeViews.Add(nodeView);
            AddElement(nodeView);
        }

        private void DrawEdges()
        {
            foreach(HNGraphEdge edge in graphEditorData.Edges)
            {
                HNGraphPortView outputPortView = null;
                HNGraphPortView inputPortView = null;
                foreach(var nodeView in nodeViews)
                {
                    foreach(var output in nodeView.OutputPortViews)
                    {
                        if(output.PortData == edge.OutputPort)
                        {
                            outputPortView = output;
                        }
                    }
                    foreach(var input in nodeView.InputPortViews)
                    {
                        if(input.PortData == edge.InputPort)
                        {
                            inputPortView = input;
                        }
                    }
                }
                if(outputPortView != null && inputPortView != null)
                {
                    HNGraphEdgeView edgeView = new HNGraphEdgeView();
                    edgeView.OutputPortView = outputPortView;
                    edgeView.InputPortView = inputPortView;
                    edgeView.EdgeData = edge;
                    AddEdgeView(outputPortView, inputPortView, edgeView);
                }
            }
        }

        private void AddEdgeView(HNGraphPortView outputPortView, HNGraphPortView inputPortView, HNGraphEdgeView edgeView)
        {
            outputPortView.ConnectToEdge(edgeView);
            inputPortView.ConnectToEdge(edgeView);
            edgeViews.Add(edgeView);
            AddElement(edgeView);
        }

        private void AddManipulator(IManipulator manipulator)
        {
            if (manipulator != null)
            {
                manipulator.target = this;
            }
        }

        private GraphViewChange RenderGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(graphEditorData, "Move Node");
                foreach (HNGraphNodeView nodeView in graphViewChange.movedElements.OfType<HNGraphNodeView>())
                {
                    nodeView.SavePosition();
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                List<HNGraphNodeView> nodeViews = graphViewChange.elementsToRemove.OfType<HNGraphNodeView>().ToList();
                if (nodeViews.Count > 0)
                {
                    Undo.RecordObject(graphEditorData, "Remove Node");
                    for (int i = nodeViews.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodeViews[i]);
                    }
                }

                List<HNGraphEdgeView> edgeViews = graphViewChange.elementsToRemove.OfType<HNGraphEdgeView>().ToList();
                if (edgeViews.Count > 0)
                {
                    Undo.RecordObject(graphEditorData, "Remove Edge");
                    for (int i = edgeViews.Count - 1; i >= 0; i--)
                    {
                        RemoveEdge(edgeViews[i]);
                    }
                }
            }

            return graphViewChange;
        }

        private void RemoveNode(HNGraphNodeView node)
        {
            graphEditorData.Nodes.Remove(node.NodeData);
            nodeViews.Remove(node);
            //AssetDatabase.Refresh();
        }

        private void RemoveEdge(HNGraphEdgeView edge)
        {
            graphEditorData.Edges.Remove(edge.EdgeData);
            edgeViews.Remove(edge);
        }


        private static string SerializeGraphElementsImplementation(IEnumerable<GraphElement> elements)
        {
            Debug.Log("Copy");

            return null;
        }
    }
}
