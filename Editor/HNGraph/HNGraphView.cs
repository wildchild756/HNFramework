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
        private Dictionary<string, HNGraphNodeView> nodeViewDict;
        private List<HNGraphEdgeView> edgeViews;
        private Dictionary<string, HNGraphEdgeView> edgeViewDict;
        private HNGraphEdgeConnectorListener edgeConnectorListener;

        public HNGraphView(HNGraphEditorWindow graphEditorWindow, HNGraphEditorData graphEditorData, HNGraphSearchWindowProvider searchWindowProvider)
        {
            this.graphEditorWindow = graphEditorWindow;
            this.graphEditorData = graphEditorData;

            nodeViews = new List<HNGraphNodeView>();
            nodeViewDict = new Dictionary<string, HNGraphNodeView>();
            edgeViews = new List<HNGraphEdgeView>();
            edgeViewDict = new Dictionary<string, HNGraphEdgeView>();

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
            HNGraphPortView outputPortView = (HNGraphPortView)edgeView.output;
            HNGraphPortView inputPortView = (HNGraphPortView)edgeView.input;
            if (outputPortView == null && inputPortView == null)
            {
                return;
            }
            Undo.RecordObject(graphEditorData, "Add Edge");
            HNGraphEdge connection = new HNGraphEdge(outputPortView, inputPortView);
            edgeView.ConnectionData = connection;
            graphEditorData.Edges.Add(connection);

            AddEdgeView(outputPortView, inputPortView, edgeView);
        }


        private void DrawNodes()
        {
            foreach (HNGraphNode node in graphEditorData.Nodes)
            {
                AddNodeView(node);
            }
        }

        private void DrawEdges()
        {
            foreach(HNGraphEdge edge in graphEditorData.Edges)
            {
                HNGraphPortView outputPortView = null;
                HNGraphPortView inputPortView = null;
                foreach(HNGraphNodeView nodeView in nodeViews)
                {
                    if(nodeView.NodeData.Guid == edge.OutputGuid)
                    {
                        if(nodeView.OutputPortViews.Count > edge.OutputPortIndex)
                        {
                            outputPortView = nodeView.OutputPortViews[edge.OutputPortIndex];
                        }
                    }
                    if(nodeView.NodeData.Guid == edge.InputGuid)
                    {
                        if(nodeView.InputPortViews.Count > edge.InputPortIndex)
                        {
                            inputPortView = nodeView.InputPortViews[edge.InputPortIndex];
                        }
                    }
                }
                if(outputPortView != null && inputPortView != null)
                {
                    HNGraphEdgeView edgeView = new HNGraphEdgeView();
                    edgeView.output = outputPortView;
                    edgeView.input = inputPortView;
                    edgeView.ConnectionData = edge;
                    AddEdgeView(outputPortView, inputPortView, edgeView);
                }
            }
        }

        private void AddNodeView(HNGraphNode node)
        {
            HNGraphNodeView nodeView = new HNGraphNodeView(node, edgeConnectorListener);
            nodeView.SetPosition(node.Position);
            nodeViews.Add(nodeView);
            nodeViewDict.Add(node.Guid, nodeView);
            AddElement(nodeView);
        }

        private void AddEdgeView(HNGraphPortView outputPortView, HNGraphPortView inputPortView, HNGraphEdgeView edgeView)
        {
            outputPortView.Connect(edgeView);
            inputPortView.Connect(edgeView);
            edgeViews.Add(edgeView);
            edgeViewDict.Add(edgeView.ConnectionData.Guid, edgeView);
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
            nodeViewDict.Remove(node.NodeData.Guid);
            nodeViews.Remove(node);
            //AssetDatabase.Refresh();
        }

        private void RemoveEdge(HNGraphEdgeView edge)
        {
            graphEditorData.Edges.Remove(edge.ConnectionData);
            edgeViewDict.Remove(edge.ConnectionData.Guid);
            edgeViews.Remove(edge);
        }


        private static string SerializeGraphElementsImplementation(IEnumerable<GraphElement> elements)
        {
            Debug.Log("Copy");

            return null;
        }
    }
}
