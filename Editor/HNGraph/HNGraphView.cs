using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using HN.Serialize;
using System.Security.Cryptography;
using Unity.VisualScripting;
using log4net.Appender;

namespace HN.Graph.Editor
{
    public class HNGraphView : GraphView
    {
        private static readonly string graphViewStyle = "Styles/HNGraphView";


        public HNGraphEditorWindow GraphEditorWindow;

        public HNGraphEditorData GraphEditorData;


        public IReadOnlyList<HNGraphNodeView> NodeViews => nodeViews;

        public IReadOnlyList<HNGraphEdgeView> ConnectionViews => edgeViews;

        public IReadOnlyList<HNGraphGroupView> GroupViews => groupViews;

        public IReadOnlyList<HNGraphStickyNoteView> StickyNoteViews => stickyNoteViews;

        public IReadOnlyList<HNGraphRelayNodeView> RelayNodeViews => relayNodeViews;

        public IReadOnlyList<HNGraphFloatingPanelView> FloatingPanelViews => floatingPanelViews;


        public SelectionChanged OnSelectionChanged;

        public GraphViewElementsCreated graphViewElementsCreated;


        private List<HNGraphNodeView> nodeViews;

        private List<HNGraphEdgeView> edgeViews;

        private List<HNGraphGroupView> groupViews;

        private List<HNGraphStickyNoteView> stickyNoteViews;

        private List<HNGraphRelayNodeView> relayNodeViews;

        private List<HNGraphFloatingPanelView> floatingPanelViews;

        private HNGraphEdgeConnectorListener edgeConnectorListener;


        public HNGraphView(HNGraphEditorWindow graphEditorWindow, HNGraphEditorData graphEditorData, HNGraphSearchWindowProvider searchWindowProvider)
        {
            this.GraphEditorWindow = graphEditorWindow;
            this.GraphEditorData = graphEditorData;

            nodeViews = new List<HNGraphNodeView>();
            edgeViews = new List<HNGraphEdgeView>();
            groupViews = new List<HNGraphGroupView>();
            stickyNoteViews = new List<HNGraphStickyNoteView>();
            relayNodeViews = new List<HNGraphRelayNodeView>();
            floatingPanelViews = new List<HNGraphFloatingPanelView>();

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
            graphViewChanged = OnGraphViewChanged;

            // DrawNodes();
            // DrawRelayNodes();
            // DrawEdges();
            // DrawGroups();
            // DrawStickyNotes();

            styleSheets.Add(Resources.Load<StyleSheet>(graphViewStyle));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            HNGraphBasePortView startPortView = startPort as HNGraphBasePortView;
            if(startPortView == null)
                return compatiblePorts;

            foreach (var nodeView in nodeViews)
            {
                List<HNGraphPortView> nodeViewPortViews = new List<HNGraphPortView>();
                if (startPortView.direction == Direction.Output)
                {
                    nodeViewPortViews = nodeView.InputPortViews.ToList();
                }
                else if (startPortView.direction == Direction.Input)
                {
                    nodeViewPortViews = nodeView.OutputPortViews.ToList();
                }
                foreach (var portView in nodeViewPortViews)
                {
                    if (startPortView.IsComptibleWith(portView))
                    {
                        compatiblePorts.Add(portView);
                    }
                }
            }

            foreach (var relayNodeView in relayNodeViews)
            {
                if (startPortView.IsComptibleWith(relayNodeView.InputPortView.RefPortView))
                {
                    compatiblePorts.Add(relayNodeView.InputPortView);
                }
                if (startPortView.IsComptibleWith(relayNodeView.OutputPortView.RefPortView))
                {
                    compatiblePorts.Add(relayNodeView.OutputPortView);
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

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);

            OnSelectionChanged?.Invoke(selection);
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);

            OnSelectionChanged?.Invoke(selection);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();

            OnSelectionChanged?.Invoke(selection);
        }


        private void BuidlGroupContextualMenu(ContextualMenuPopulateEvent evt, int menuPosition = -1)
        {
            if(menuPosition == -1)
                menuPosition = evt.menu.MenuItems().Count;

            var target = evt.currentTarget as HNGraphView;
            if(target == null)
                return;

            Vector2 position = target.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            HNGraphGroup groupData = new HNGraphGroup(GraphEditorData, "New Group");
            groupData.Initialize(position);
            evt.menu.InsertAction(menuPosition, "Create Group", (e) => AddGroup(groupData), DropdownMenuAction.AlwaysEnabled);
        }

        private void BuildStickNoteContextualMenu(ContextualMenuPopulateEvent evt, int menuPosition = -1)
        {
            if(menuPosition == -1)
                menuPosition = evt.menu.MenuItems().Count;
            
            var target = evt.currentTarget as HNGraphView;
            if(target == null)
                return;

            Vector2 position = target.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            HNGraphStickyNote stickyNoteData = new HNGraphStickyNote(GraphEditorData, "Create Note");
            stickyNoteData.Initialize(position);
            evt.menu.InsertAction(menuPosition, "Create Sticky Note", (e) => AddStickyNote(stickyNoteData), DropdownMenuAction.AlwaysEnabled);
        }


        public void AddNode(HNGraphNode nodeData)
        {
            Undo.RecordObject(GraphEditorData, "Add Node");
            GraphEditorData.AddNode(nodeData);

            AddNodeView(nodeData, out HNGraphNodeView nodeView);

            graphViewElementsCreated(new HNGraphViewCreateElements(nodeView));
        }

        public void AddEdge(HNGraphEdgeView edgeView)
        {
            if (edgeView.output == null && edgeView.input == null)
                return;

            Undo.RecordObject(GraphEditorData, "Add Edge");

            HNGraphBasePortView outputBasePortView = edgeView.OutputPortView;
            HNGraphPortView refOutputPortView = outputBasePortView.RefPortView;
            
            HNGraphBasePortView inputBasePortView = edgeView.InputPortView;
            HNGraphPortView refInputPortView = inputBasePortView.RefPortView;

            HNGraphEdge edgeData = new HNGraphEdge(GraphEditorData, outputBasePortView.PortData, inputBasePortView.PortData);
            edgeData.Initialize();
            GraphEditorData.AddEdge(edgeData);
            edgeView.Initialize(edgeData, outputBasePortView, inputBasePortView);

            UpdateOutputRefPortView(edgeView.OutputPortView, refOutputPortView);
            UpdateInputRefPortView(edgeView.InputPortView, refInputPortView);
            
            AddEdgeView(edgeData, ref edgeView);

            OnEdgeAdded(edgeView);
            graphViewElementsCreated(new HNGraphViewCreateElements(edgeView));
        }

        public void AddGroup(HNGraphGroup group)
        {
            Undo.RecordObject(GraphEditorData, "Add Group");
            GraphEditorData.AddGroup(group);
            
            AddGroupView(group, out HNGraphGroupView groupView);
            
            graphViewElementsCreated(new HNGraphViewCreateElements(groupView));
        }

        public void AddStickyNote(HNGraphStickyNote stickyNoteData)
        {
            Undo.RecordObject(GraphEditorData, "Add Sticky Note");
            GraphEditorData.AddStickyNote(stickyNoteData);

            AddStickyNoteView(stickyNoteData, out HNGraphStickyNoteView stickyNoteView);
            
            graphViewElementsCreated(new HNGraphViewCreateElements(stickyNoteView));
        }

        public void AddRelayNode(HNGraphEdgeView edgeView, Vector2 mousePos)
        {
            Undo.RecordObject(GraphEditorData, "Add Relay Node");

            HNGraphEdge edgeData = edgeView.EdgeData;
            HNGraphRelayNode relayNodeData = new HNGraphRelayNode(GraphEditorData, edgeData);
            relayNodeData.Initialize(mousePos);
            Debug.Log("mouse pos:" + mousePos);
            GraphEditorData.AddRelayNode(relayNodeData);

            AddRelayNodeView(relayNodeData, out HNGraphRelayNodeView relayNodeView);
            relayNodeView.CreateRelayNodeOnEdge(edgeView, relayNodeView);

            graphViewElementsCreated(new HNGraphViewCreateElements(relayNodeView));
        }

        public void DeleteRelayNode(HNGraphRelayNodeView relayNodeView)
        {
            HNGraphBasePortView outputPortView = null, inputPortView = null;

            HNGraphBasePortView relayNodeInputPortView = relayNodeView.InputPortView;
            if(relayNodeInputPortView.EdgeViews.Count > 0)
            {
                HNGraphEdgeView inputEdgeView = relayNodeInputPortView.EdgeViews[0];
                outputPortView = inputEdgeView.GetAnotherPort(relayNodeInputPortView);
                RemoveElement(inputEdgeView);
                GraphEditorData.RemoveEdge(inputEdgeView.EdgeData);
            }

            HNGraphBasePortView relayNodeOutputPortView = relayNodeView.OutputPortView;
            if(relayNodeOutputPortView.EdgeViews.Count > 0)
            {
                HNGraphEdgeView outputEdgeView = relayNodeOutputPortView.EdgeViews[0];
                inputPortView = outputEdgeView.GetAnotherPort(relayNodeOutputPortView);
                RemoveElement(outputEdgeView);
                GraphEditorData.RemoveEdge(outputEdgeView.EdgeData);
            }

            RemoveElement(relayNodeView);
            GraphEditorData.RemoveRelayNode(relayNodeView.RelayNodeData);

            if(outputPortView != null && inputPortView != null)
            {
                HNGraphEdge newEdge = new HNGraphEdge(GraphEditorData, outputPortView.PortData, inputPortView.PortData);
                newEdge.Initialize();
                HNGraphEdgeView newEdgeView = new HNGraphEdgeView(this);
                newEdgeView.Initialize(newEdge, outputPortView, inputPortView);
                AddElement(newEdgeView);
            }

            EditorUtility.SetDirty(GraphEditorWindow);
        }

        public void DrawFloatingPanelView(HNGraphFloatingPanelView floatingPanelView)
        {
            HNGraphNodeView selectedNode = null;
            for(int i = 0; i < selection.Count; i++)
            {
                selectedNode = selection[i] as HNGraphNodeView;
                if(selectedNode != null)
                    break;
            }
            
            floatingPanelViews.Add(floatingPanelView);
            Add(floatingPanelView);

            OnSelectionChanged?.Invoke(selection);
        }

        public void CloseFloatingPanel(Type floatingPanelType)
        {
            foreach(HNGraphFloatingPanelView floatingPanelView in floatingPanelViews)
            {
                if(floatingPanelView.FloatingPanelData.GetType() == floatingPanelType)
                {
                    RemoveFloatingPanelView(floatingPanelView);
                    break;
                }
            }
        }


        private void UpdateOutputRefPortView(HNGraphBasePortView outputBasePortView, HNGraphPortView newOutputPortView)
        {
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

        private void DrawNodes()
        {
            foreach(var nodeData in GraphEditorData.Nodes.Values)
            {
                AddNodeView(nodeData, out HNGraphNodeView nodeView);
            }
        }

        private void DrawRelayNodes()
        {
            var relayNodeDataList = GraphEditorData.RelayNodes.Values.ToList();
            for(int i = 0; i < relayNodeDataList.Count; i++)
            {
                AddRelayNodeView(relayNodeDataList[i], out HNGraphRelayNodeView relayNodeView);
            }
        }

        private void DrawEdges()
        {
            foreach(var edgeData in GraphEditorData.Edges.Values)
            {
                HNGraphEdgeView edgeView = null;
                AddEdgeView(edgeData, ref edgeView);
            }

        }

        private void DrawGroups()
        {
            foreach(var groupData in GraphEditorData.Groups.Values)
            {
                AddGroupView(groupData, out HNGraphGroupView groupView);
            }
        }

        private void DrawStickyNotes()
        {
            foreach(var stickyNoteData in GraphEditorData.StickyNotes.Values)
            {
                AddStickyNoteView(stickyNoteData, out HNGraphStickyNoteView stickyNoteView);
            }

        }

        // private void AddConnectionViewFromData(HNGraphEdge connectionData)
        // {
        //     HNGraphBasePortView output = null;
        //     HNGraphBasePortView input = null;
        //     foreach (var nodeView in nodeViews)
        //     {
        //         foreach (var outputPortView in nodeView.OutputPortViews)
        //         {
        //             if (outputPortView.PortData.Guid == connectionData.OutputPort.Guid)
        //             {
        //                 output = outputPortView;
        //                 break;
        //             }
        //         }
        //         if(output != null)
        //             break;
        //     }
        //     foreach (var nodeView in nodeViews)
        //     {
        //         foreach (var inputPortView in nodeView.InputPortViews)
        //         {
        //             if (inputPortView.PortData.Guid == connectionData.InputPort.Guid)
        //             {
        //                 input = inputPortView;
        //                 break;
        //             }
        //         }
        //         if(input != null)
        //             break;
        //     }
                        
        //     if (output != null && input != null)
        //     {
        //         HNGraphEdgeView connectionView = new HNGraphEdgeView(this, connectionData);
        //         connectionView.Initialize(output, input);
        //         edgeViews.Add(connectionView);
        //         if(connectionData.RelayNodeGuids.Count == 0)
        //         {
        //             HNGraphEdgeView edgeView = new HNGraphEdgeView(this);
        //             edgeView.Initialize(connectionView, output, input);
        //             connectionView.AddEdgeView(0, edgeView);
        //             AddElement(edgeView);
        //         }
        //         else
        //         {
        //             HNGraphBasePortView lastRelayNodeOutputPortView = output;
        //             var relayNodeGuidList = connectionData.RelayNodeGuids;
        //             for(int i = 0; i < relayNodeGuidList.Count; i++)
        //             {
        //                 HNGraphRelayNodeView currentRelayNodeView = GetRelayNodeViewFromGuid(relayNodeGuidList[i]);
        //                 HNGraphEdgeView edgeView = new HNGraphEdgeView(this);
        //                 edgeView.Initialize(connectionView, lastRelayNodeOutputPortView, currentRelayNodeView.InputPortView);
        //                 connectionView.AddEdgeView(i, edgeView);
        //                 AddElement(edgeView);
        //                 connectionView.AddRelayNodeView(i, currentRelayNodeView);
        //                 lastRelayNodeOutputPortView = currentRelayNodeView.OutputPortView;
        //             }
        //             HNGraphEdgeView lastEdgeView = new HNGraphEdgeView(this);
        //             lastEdgeView.Initialize(connectionView, lastRelayNodeOutputPortView, input);
        //             connectionView.AddEdgeView(relayNodeGuidList.Count, lastEdgeView);
        //             AddElement(lastEdgeView);
        //         }
        //     }

        // }

        private void AddNodeView(HNGraphNode nodeData, out HNGraphNodeView nodeView)
        {
            nodeView = new HNGraphNodeView(this, nodeData, edgeConnectorListener);
            nodeView.Initialize();
            nodeViews.Add(nodeView);
            AddElement(nodeView);
        }

        private void AddEdgeView(HNGraphEdge edgeData, ref HNGraphEdgeView edgeView)
        {
            if(edgeView == null)
                edgeView = new HNGraphEdgeView(this);
            edgeView.Initialize(edgeData, edgeView.OutputPortView, edgeView.InputPortView);
            edgeViews.Add(edgeView);
            AddElement(edgeView);
        }

        private void AddGroupView(HNGraphGroup groupData, out HNGraphGroupView groupView)
        {
            groupView = new HNGraphGroupView(this, groupData);
            groupView.Initialize();
            groupViews.Add(groupView);
            AddElement(groupView);
        }

        private void AddStickyNoteView(HNGraphStickyNote stickyNoteData, out HNGraphStickyNoteView stickyNoteView)
        {
            stickyNoteView = new HNGraphStickyNoteView(this, stickyNoteData);
            stickyNoteView.Initialize();
            stickyNoteViews.Add(stickyNoteView);
            AddElement(stickyNoteView);
        }

        private void AddRelayNodeView(HNGraphRelayNode relayNodeData, out HNGraphRelayNodeView relayNodeView)
        {
            relayNodeView = new HNGraphRelayNodeView(this, relayNodeData, edgeConnectorListener);
            relayNodeView.Initialize();
            Debug.Log("relay node view:" + relayNodeView);
            relayNodeViews.Add(relayNodeView);
            AddElement(relayNodeView);
        }

        private void OnEdgeAdded(HNGraphEdgeView connectionView)
        {
            connectionView?.InputPortView?.OwnerNodeView?.BaseNodeData.OnConnectionAdded(connectionView.InputPortView.PortData.Guid);
            connectionView?.OutputPortView?.OwnerNodeView?.BaseNodeData.OnConnectionAdded(connectionView.OutputPortView.PortData.Guid);
        }

        private void AddManipulator(IManipulator manipulator)
        {
            if (manipulator != null)
            {
                manipulator.target = this;
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
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

                List<HNGraphRelayNodeView> relayNodeViews = graphViewChange.movedElements.OfType<HNGraphRelayNodeView>().ToList();
                if(relayNodeViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Move Relay Node");
                    for(int i = 0; i < relayNodeViews.Count; i++)
                    {
                        relayNodeViews[i].SavePosition();
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

                List<HNGraphRelayNodeView> relayNodeViews = graphViewChange.elementsToRemove.OfType<HNGraphRelayNodeView>().ToList();
                if(relayNodeViews.Count > 0)
                {
                    Undo.RecordObject(GraphEditorData, "Remove Relay Node");
                    for(int i = relayNodeViews.Count - 1; i >= 0; i--)
                    {
                        DeleteRelayNode(relayNodeViews[i]);
                    }
                }
            }

            return graphViewChange;
        }

        private void RemoveNode(HNGraphNodeView nodeView)
        {
            foreach(var portView in nodeView.InputPortViews)
            {
                var edgeGuids = portView.PortData.EdgeGuids;
                foreach(var edgeGuid in edgeGuids)
                {
                    HNGraphEdgeView edgeView = GetEdgeViewFromGuid(edgeGuid);
                    RemoveEdge(edgeView);
                }
            }
            GraphEditorData.RemoveNode(nodeView.NodeData);
            RemoveElement(nodeView);
            nodeViews.Remove(nodeView);
        }

        private void RemoveEdge(HNGraphEdgeView edgeView)
        {
            if(edgeView == null)
                return;    

            UpdateOutputRefPortView(edgeView.OutputPortView, null);
            UpdateInputRefPortView(edgeView.InputPortView, null);
            edgeView.DisconnectOutput();
            edgeView.DisconnectInput();
            edgeViews.Remove(edgeView);
        }

        private void RemoveGroup(HNGraphGroupView groupView)
        {
            GraphEditorData.RemoveGroup(groupView.GroupData);
            RemoveElement(groupView);
            groupViews.Remove(groupView);
        }

        private void RemoveStickyNote(HNGraphStickyNoteView stickyNoteView)
        {
            GraphEditorData.RemoveStickyNote(stickyNoteView.StickyNoteData);
            RemoveElement(stickyNoteView);
            stickyNoteViews.Remove(stickyNoteView);
        }

        private void RemoveFloatingPanelView(HNGraphFloatingPanelView floatingPanelView)
        {
            RemoveElement(floatingPanelView);
            floatingPanelViews.Remove(floatingPanelView);
            floatingPanelView.Dispose();
        }


        private string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        {
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
            HNGraphCopyPasteData data = new HNGraphCopyPasteData();
            Json.DeserializeFromString(data, serializedData);
            
            Undo.RecordObject(GraphEditorData, operationName);

            foreach(var nodeData in data.serializedNodes)
            {
                nodeData.Initialize(nodeData.GetLayout().position + new Vector2(20, 20));
                GraphEditorData.AddNode(nodeData);
                AddNodeView(nodeData, out HNGraphNodeView nodeView);
            }
            
            foreach(var nodeData in data.serializedNodes)
            {
                foreach(var inputPort in nodeData.InputPorts.Values)
                {
                    if(inputPort == null)
                    {
                        continue;
                    }

                    var edges = inputPort.EdgeGuids.ToList();
                    foreach(var edge in edges)
                    {
                        HNGraphBasePort connectPort = GraphEditorData.GetEdge(edge)?.OutputPort;
                        if(connectPort != null && connectPort.PortCapacity != HNGraphBasePort.Capacity.Single)
                        {
                            HNGraphEdge edgeData = new HNGraphEdge(GraphEditorData, connectPort, inputPort);
                            edgeData.Initialize();
                            HNGraphEdgeView edgeView = null;
                            AddEdgeView(edgeData, ref edgeView);
                        }
                    }
                }
            }

            foreach(var stickyNoteData in data.serializedStickyNotes)
            {
                stickyNoteData.Initialize(stickyNoteData.GetLayout().position + new Vector2(20, 20));
                GraphEditorData.AddStickyNote(stickyNoteData);
                AddStickyNoteView(stickyNoteData, out HNGraphStickyNoteView stickyNoteView);
            }

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

        private HNGraphEdgeView GetEdgeViewFromGuid(string guid)
        {
            foreach(var edgeView in edgeViews)
            {
                if(guid == edgeView.EdgeData.Guid)
                    return edgeView;
            }
            return null;
        }

        private HNGraphGroupView GetGroupViewFromGuid(string guid)
        {
            foreach(var groupView in groupViews)
            {
                if(guid == groupView.GroupData.Guid)
                    return groupView;
            }
            return null;
        }

        private HNGraphStickyNoteView GetStickyNoteViewFromGuid(string guid)
        {
            foreach(var stickyNoteView in stickyNoteViews)
            {
                if(guid == stickyNoteView.StickyNoteData.Guid)
                    return stickyNoteView;
            }
            return null;
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
    
    
        public delegate void SelectionChanged(List<ISelectable> selection);

        public delegate HNGraphViewCreateElements GraphViewElementsCreated(HNGraphViewCreateElements graphViewChange);
    }

}
