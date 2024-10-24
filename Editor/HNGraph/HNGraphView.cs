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
        private HNGraphSearchWindowProvider searchWindowProvider;
        private HNGraphEdgeConnectionListener edgeConnectorListener;

        public HNGraphView(HNGraphEditorWindow graphEditorWindow, HNGraphEditorData graphEditorData)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/HNRenderGraphView"));

            this.graphEditorWindow = graphEditorWindow;
            this.graphEditorData = graphEditorData;

            nodeViews = new List<HNGraphNodeView>();
            nodeViewDict = new Dictionary<string, HNGraphNodeView>();
            edgeViews = new List<HNGraphEdgeView>();
            edgeViewDict = new Dictionary<string, HNGraphEdgeView>();

            serializeGraphElements = SerializeGraphElementsImplementation;

            //AddManipulator(new ContentDragger());
            AddManipulator(new SelectionDragger());
            AddManipulator(new RectangleSelector());
            AddManipulator(new ClickSelector());

            GridBackground gridBackground = new GridBackground();
            Insert(0, gridBackground);
            gridBackground.StretchToParentSize();

            SetupZoom(0.05f, 8);

            searchWindowProvider = ScriptableObject.CreateInstance<HNGraphSearchWindowProvider>();
            searchWindowProvider.graph = this;
            nodeCreationRequest += context =>
            {
                //searchWindowProvider.target = (VisualElement)focusController.focusedElement;
                Vector2 pos = context.screenMousePosition;
                var searchWindowContext = new SearchWindowContext(pos, 0f, 0f);
                SearchWindow.Open(searchWindowContext, searchWindowProvider);

            };

            edgeConnectorListener = new HNGraphEdgeConnectionListener();
            graphViewChanged = RenderGraphViewChanged;

            DrawNodes();
            //DrawEdges();
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
            graphEditorData.Connection.Add(connection);

            AddEdgeView(outputPortView, inputPortView, edgeView);
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
            graphEditorData.Connection.Remove(edge.ConnectionData);
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
