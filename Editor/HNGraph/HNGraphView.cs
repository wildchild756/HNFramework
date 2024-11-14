using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using HN.Serialize;

namespace HN.Graph.Editor
{
    public class HNGraphView : GraphView
    {
        public HNGraphEditorWindow GraphEditorWindow;
        public HNGraphEditorData GraphEditorData;

        private List<HNGraphNodeView> nodeViews;
        private List<HNGraphEdgeView> edgeViews;
        private List<HNGraphGroupView> groupViews;
        private List<HNGraphStickyNoteView> stickyNoteViews;
        private HNGraphEdgeConnectorListener edgeConnectorListener;

        public HNGraphView(HNGraphEditorWindow graphEditorWindow, HNGraphEditorData graphEditorData, HNGraphSearchWindowProvider searchWindowProvider)
        {
            this.GraphEditorWindow = graphEditorWindow;
            this.GraphEditorData = graphEditorData;

            nodeViews = new List<HNGraphNodeView>();
            edgeViews = new List<HNGraphEdgeView>();
            groupViews = new List<HNGraphGroupView>();
            stickyNoteViews = new List<HNGraphStickyNoteView>();

            serializeGraphElements = SerializeGraphElementsCallback;
            canPasteSerializedData = CanPasteSerializedDataCallback;
            unserializeAndPaste = UnserializeAndPasteCallback;

            AddManipulator(new ContentDragger());
            AddManipulator(new ContentDragger());
            AddManipulator(new SelectionDragger());
            AddManipulator(new RectangleSelector());

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
            DrawGroups();
            DrawStickyNotes();

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

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            BuidlGroupContextualMenu(evt, 1);
            BuildStickNoteContextualMenu(evt, 2);
        }

        private void BuidlGroupContextualMenu(ContextualMenuPopulateEvent evt, int menuPosition = -1)
        {
            if(menuPosition == -1)
            {
                menuPosition = evt.menu.MenuItems().Count;
            }

            var target = evt.currentTarget as HNGraphView;
            if(target == null)
            {
                return;
            }
            Vector2 position = target.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            evt.menu.InsertAction(menuPosition, "Create Group", (e) => AddGroup(new HNGraphGroup(GraphEditorData, "New Group", position)), DropdownMenuAction.AlwaysEnabled);
        }

        private void BuildStickNoteContextualMenu(ContextualMenuPopulateEvent evt, int menuPosition = -1)
        {
            if(menuPosition == -1)
            {
                menuPosition = evt.menu.MenuItems().Count;
            }
            
            var target = evt.currentTarget as HNGraphView;
            if(target == null)
            {
                return;
            }
            Vector2 position = target.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            evt.menu.InsertAction(menuPosition, "Create Sticky Note", (e) => AddStickyNote(new HNGraphStickyNote(GraphEditorData, "Create Note", position)), DropdownMenuAction.AlwaysEnabled);
        }

        public void AddNode(HNGraphNode nodeData)
        {
            Undo.RecordObject(GraphEditorData, "Add Node");
            GraphEditorData.AddNode(nodeData);

            AddNodeView(nodeData);
        }

        public void AddEdge(HNGraphEdgeView edgeView)
        {
            if (edgeView.output == null && edgeView.input == null)
            {
                return;
            }
            Undo.RecordObject(GraphEditorData, "Add Edge");
            HNGraphPortView outputPort = (HNGraphPortView)edgeView.output;
            HNGraphPortView inputPort = (HNGraphPortView)edgeView.input;
            HNGraphEdge connection = new HNGraphEdge(GraphEditorData, outputPort.PortData, inputPort.PortData);
            edgeView.EdgeData = connection;
            GraphEditorData.AddEdge(connection);

            AddEdgeViewToGraph((HNGraphPortView)edgeView.output, (HNGraphPortView)edgeView.input, edgeView);

            OnEdgeAdded(edgeView);
        }

        public void AddGroup(HNGraphGroup group)
        {
            Undo.RecordObject(GraphEditorData, "Add Group");
            GraphEditorData.AddGroup(group);
            
            AddGroupView(group);
        }

        public void AddStickyNote(HNGraphStickyNote stickyNoteData)
        {
            Undo.RecordObject(GraphEditorData, "Add Sticky Note");
            GraphEditorData.AddStickyNote(stickyNoteData);
            AddStickyNoteView(stickyNoteData);
        }

        public void CreateRelayNodeOnEdge(HNGraphEdgeView edgeView, Vector2 mousePos)
        {
            Undo.RecordObject(GraphEditorData, "Add Relay Node");
            HNGraphRelayNode relayNodeData = new HNGraphRelayNode(edgeView.EdgeData, mousePos);
            GraphEditorData.AddRelayNode(relayNodeData);
            
        }


        private void DrawNodes()
        {
            var nodes = GraphEditorData.GetNodesEnumerator();
            while(nodes.MoveNext())
            {
                HNGraphNode node = nodes.Current as HNGraphNode;
                if(node == null)
                {
                    continue;
                }
                AddNodeView(nodes.Current as HNGraphNode);
            }
        }

        private void DrawEdges()
        {
            var edges = GraphEditorData.GetEdgesEnumerator();
            while(edges.MoveNext())
            {
                HNGraphEdge edge = edges.Current as HNGraphEdge;
                if(edge == null)
                {
                    continue;
                }

                AddEdgeViewFromData(edge);
            }

        }

        private void AddEdgeViewFromData(HNGraphEdge edgeData)
        {
            HNGraphPortView output = null;
            HNGraphPortView input = null;
            foreach (var nodeView in nodeViews)
            {
                foreach (var outputPortView in nodeView.OutputPortViews)
                {
                    if (outputPortView.PortData.Guid == edgeData.OutputPort.Guid)
                    {
                        output = outputPortView;
                    }
                }
                foreach (var inputPortView in nodeView.InputPortViews)
                {
                    if (inputPortView.PortData.Guid == edgeData.InputPort.Guid)
                    {
                        input = inputPortView;
                    }
                }
            }

            if (output != null && input != null)
            {
                HNGraphEdgeView edgeView = new HNGraphEdgeView(this);
                edgeView.output = output;
                edgeView.input = input;
                edgeView.EdgeData = edgeData;
                AddEdgeViewToGraph(output, input, edgeView);
            }
        }

        private void DrawGroups()
        {
            var groups = GraphEditorData.GetGroupsEnumerator();
            while(groups.MoveNext())
            {
                HNGraphGroup group = groups.Current as HNGraphGroup;
                if(group == null)
                {
                    continue;
                }

                AddGroupView(group);
            }
        }

        private void DrawStickyNotes()
        {
            var stickyNotes = GraphEditorData.GetStickyNoteEnumerator();
            while(stickyNotes.MoveNext())
            {
                HNGraphStickyNote stickyNote = stickyNotes.Current as HNGraphStickyNote;
                if(stickyNote == null)
                {
                    continue;
                }

                AddStickyNoteView(stickyNote);
            }

        }

        private void AddNodeView(HNGraphNode nodeData)
        {
            HNGraphNodeView nodeView = new HNGraphNodeView(this, nodeData, edgeConnectorListener);
            nodeView.SetPosition(nodeData.GetLayout());
            nodeViews.Add(nodeView);
            AddElement(nodeView);
        }

        private void AddEdgeViewToGraph(HNGraphPortView output, HNGraphPortView input, HNGraphEdgeView edgeView)
        {
            output.ConnectToEdge(edgeView);
            input.ConnectToEdge(edgeView);
            edgeViews.Add(edgeView);
            AddElement(edgeView);
        }

        private void AddGroupView(HNGraphGroup groupData)
        {
            HNGraphGroupView groupView = new HNGraphGroupView(groupData);
            groupView.SetPosition(groupData.GetLayout());
            groupViews.Add(groupView);
            AddElement(groupView);
            AddElementsToGroup(groupView);
            AddSelectionsToGroup(groupView);
        }

        private void AddStickyNoteView(HNGraphStickyNote stickyNoteData)
        {
            HNGraphStickyNoteView stickyNoteView = new HNGraphStickyNoteView(stickyNoteData);
            stickyNoteView.SetPosition(stickyNoteData.GetLayout());
            stickyNoteViews.Add(stickyNoteView);
            AddElement(stickyNoteView);
        }

        private void OnEdgeAdded(HNGraphEdgeView edgeView)
        {
            edgeView?.InputPortView?.OwnerNodeView?.BaseNodeData.OnEdgeAdded(edgeView.InputPortView.PortData.Guid);
            edgeView?.OutputPortView?.OwnerNodeView?.BaseNodeData.OnEdgeAdded(edgeView.OutputPortView.PortData.Guid);
        }

        private void AddElementsToGroup(HNGraphGroupView groupView)
        {
            HNGraphGroup groupData = groupView.GroupData;
            foreach(var nodeGuid in groupData.InnerNodeGuids)
            {
                if(string.IsNullOrEmpty(nodeGuid))
                {
                    continue;
                }

                foreach(var nodeView in nodeViews)
                {
                    if(nodeView.BaseNodeData.Guid == nodeGuid)
                    {
                        groupView.AddElement(nodeView);
                    }
                }
            }
        }

        private void AddSelectionsToGroup(HNGraphGroupView groupView)
        {
            foreach(var selectedNode in selection)
            {
                if(selectedNode is HNGraphNodeView)
                {
                    if(groupViews.Exists(x => x.ContainsElement(selectedNode as HNGraphNodeView)))
                    {
                        continue;
                    }
                    HNGraphNodeView selectedNodeView = selectedNode as HNGraphNodeView;
                    groupView.AddElement(selectedNodeView);
                    groupView.GroupData.AddNode(selectedNodeView.BaseNodeData.Guid);
                }
            }
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
                List<HNGraphNodeView> nodeViews = graphViewChange.movedElements.OfType<HNGraphNodeView>().ToList();
                if(nodeViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Move Node");
                    for(int i = 0; i < nodeViews.Count; i++)
                    {
                        nodeViews[i].SavePosition();
                    }
                }

                List<HNGraphEdgeView> edgeViews = graphViewChange.movedElements.OfType<HNGraphEdgeView>().ToList();
                if(edgeViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Move Edge");
                    for(int i = edgeViews.Count - 1; i >= 0; i--)
                    {
                        RemoveEdge(edgeViews[i]);
                    }
                }

                List<HNGraphGroupView> groupViews = graphViewChange.movedElements.OfType<HNGraphGroupView>().ToList();
                if(groupViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Move Group");
                    for(int i = 0; i < groupViews.Count; i++)
                    {
                        groupViews[i].SavePosition();
                    }
                }

                List<HNGraphStickyNoteView> stickyNoteViews = graphViewChange.movedElements.OfType<HNGraphStickyNoteView>().ToList();
                if(stickyNoteViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Move Sticky Note");
                    for(int i = 0; i < stickyNoteViews.Count; i++)
                    {
                        stickyNoteViews[i].SavePosition();
                    }
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                List<HNGraphNodeView> nodeViews = graphViewChange.elementsToRemove.OfType<HNGraphNodeView>().ToList();
                if (nodeViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Remove Node");
                    for (int i = nodeViews.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodeViews[i]);
                    }
                }

                List<HNGraphEdgeView> edgeViews = graphViewChange.elementsToRemove.OfType<HNGraphEdgeView>().ToList();
                if (edgeViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Remove Edge");
                    for (int i = edgeViews.Count - 1; i >= 0; i--)
                    {
                        RemoveEdge(edgeViews[i]);
                    }
                }

                List<HNGraphGroupView> groupViews = graphViewChange.elementsToRemove.OfType<HNGraphGroupView>().ToList();
                if(groupViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Remove Group");
                    for(int i = groupViews.Count - 1; i >= 0; i--)
                    {
                        RemoveGroup(groupViews[i]);
                    }
                }

                List<HNGraphStickyNoteView> stickyNoteViews = graphViewChange.elementsToRemove.OfType<HNGraphStickyNoteView>().ToList();
                if(stickyNoteViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Remove Sticky Note");
                    for(int i = stickyNoteViews.Count - 1; i >= 0; i--)
                    {
                        RemoveStickyNote(stickyNoteViews[i]);
                    }
                }
            }

            return graphViewChange;
        }

        private void RemoveNode(HNGraphNodeView nodeView)
        {
            GraphEditorData.RemoveNode(nodeView.NodeData);
            nodeView.BaseNodeData.Dispose();
            RemoveElement(nodeView);
            nodeViews.Remove(nodeView);
            //AssetDatabase.Refresh();
        }

        private void RemoveEdge(HNGraphEdgeView edgeView)
        {
            GraphEditorData.RemoveEdge(edgeView.EdgeData);
            edgeView.EdgeData.Dispose();
            RemoveElement(edgeView);
            edgeViews.Remove(edgeView);
        }

        private void RemoveGroup(HNGraphGroupView groupView)
        {
            GraphEditorData.RemoveGroup(groupView.GroupData);
            groupView.GroupData.Dispose();
            RemoveElement(groupView);
            groupViews.Remove(groupView);
        }

        private void RemoveStickyNote(HNGraphStickyNoteView stickyNoteView)
        {
            GraphEditorData.RemoveStickyNote(stickyNoteView.StickyNoteData);
            stickyNoteView.StickyNoteData.Dispose();
            RemoveElement(stickyNoteView);
            stickyNoteViews.Remove(stickyNoteView);
        }


        private string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        {
            Debug.Log("Copy");

            HNGraphCopyPasteData data = new HNGraphCopyPasteData();

            foreach(HNGraphNodeView nodeView in elements.Where(e => e is HNGraphNodeView))
            {
                HNGraphNode nodeData = nodeView.NodeData;
                if(!data.serializedNodes.Contains(nodeData))
                {
                    data.serializedNodes.Add(nodeData);
                }
            }

            foreach(HNGraphStickyNoteView stickyNoteView in elements.Where(e => e is HNGraphStickyNoteView))
            {
                HNGraphStickyNote stickyNoteData = stickyNoteView.StickyNoteData;
                if(!data.serializedStickyNotes.Contains(stickyNoteData))
                {
                    data.serializedStickyNotes.Add(stickyNoteData);
                }
            }

            return Json.Serialize(data);
        }

        private bool CanPasteSerializedDataCallback(string serializedData)
        {
            return true;
        }

        private void UnserializeAndPasteCallback(string operationName, string serializedData)
        {
            Debug.Log("Paste");

            HNGraphCopyPasteData data = new HNGraphCopyPasteData();
            Json.DeserializeFromString(data, serializedData);
            
            Undo.RecordObject(GraphEditorData, operationName);

            foreach(var nodeData in data.serializedNodes)
            {
                nodeData.OnCreate(nodeData.GetLayout().position + new Vector2(20, 20));
                GraphEditorData.AddNode(nodeData);
                AddNodeView(nodeData);
            }
            
            foreach(var nodeData in data.serializedNodes)
            {
                foreach(var inputPort in nodeData.InputPorts.Values)
                {
                    if(inputPort == null)
                    {
                        continue;
                    }

                    var edges = inputPort.Edges.ToArray();
                    foreach(var edge in edges)
                    {
                        HNGraphPort connectPort = edge.OutputPort;
                        if(connectPort != null && connectPort.PortCapacity != HNGraphPort.Capacity.Single)
                        {
                            HNGraphEdge connection = new HNGraphEdge(GraphEditorData, connectPort, inputPort);
                            AddEdgeViewFromData(connection);
                        }
                    }
                }
            }

            
            foreach(var stickyNoteData in data.serializedStickyNotes)
            {
                stickyNoteData.OnCreate(stickyNoteData.GetLayout().position + new Vector2(20, 20));
                GraphEditorData.AddStickyNote(stickyNoteData);
                AddStickyNoteView(stickyNoteData);
            }

        }
    }
}
