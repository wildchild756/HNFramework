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

namespace HN.Graph.Editor
{
    public class HNGraphView : GraphView
    {
        public HNGraphEditorWindow GraphEditorWindow;

        public HNGraphEditorData GraphEditorData;


        public IReadOnlyList<HNGraphNodeView> NodeViews => nodeViews;

        public IReadOnlyList<HNGraphConnectionView> ConnectionViews => connectionViews;

        public IReadOnlyList<HNGraphGroupView> GroupViews => groupViews;

        public IReadOnlyList<HNGraphStickyNoteView> StickyNoteViews => stickyNoteViews;

        public IReadOnlyList<HNGraphRelayNodeView> RelayNodeViews => relayNodeViews;


        private List<HNGraphNodeView> nodeViews;

        private List<HNGraphConnectionView> connectionViews;

        private List<HNGraphGroupView> groupViews;

        private List<HNGraphStickyNoteView> stickyNoteViews;

        private List<HNGraphRelayNodeView> relayNodeViews;

        private HNGraphEdgeConnectorListener edgeConnectorListener;


        public HNGraphView(HNGraphEditorWindow graphEditorWindow, HNGraphEditorData graphEditorData, HNGraphSearchWindowProvider searchWindowProvider)
        {
            this.GraphEditorWindow = graphEditorWindow;
            this.GraphEditorData = graphEditorData;

            nodeViews = new List<HNGraphNodeView>();
            connectionViews = new List<HNGraphConnectionView>();
            groupViews = new List<HNGraphGroupView>();
            stickyNoteViews = new List<HNGraphStickyNoteView>();
            relayNodeViews = new List<HNGraphRelayNodeView>();

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
            DrawRelayNodes();
            DrawConnections();
            DrawGroups();
            DrawStickyNotes();

            styleSheets.Add(Resources.Load<StyleSheet>("Styles/HNGraphView"));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            HNGraphPortView startPortView = startPort as HNGraphPortView;
            if(startPortView == null || startPortView.OwnerNodeView is not HNGraphNodeView)
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
        }

        public void AddConnection(HNGraphEdgeView edgeView)
        {
            if (edgeView.output == null && edgeView.input == null)
                return;

            Undo.RecordObject(GraphEditorData, "Add Connection");

            HNGraphPortView outputPort = (HNGraphPortView)edgeView.output;
            HNGraphPortView inputPort = (HNGraphPortView)edgeView.input;

            HNGraphConnection connectionData = new HNGraphConnection(GraphEditorData, outputPort.PortData, inputPort.PortData);
            connectionData.Initialize();
            GraphEditorData.AddConnection(connectionData);

            AddConnectionView(connectionData, edgeView, out HNGraphConnectionView connectionView);
            edgeView.ConnectionView = connectionView;

            OnConnectionAdded(connectionView);

            AddElement(edgeView);
        }

        public void AddGroup(HNGraphGroup group)
        {
            Undo.RecordObject(GraphEditorData, "Add Group");
            GraphEditorData.AddGroup(group);
            
            AddGroupView(group, out HNGraphGroupView groupView);
        }

        public void AddStickyNote(HNGraphStickyNote stickyNoteData)
        {
            Undo.RecordObject(GraphEditorData, "Add Sticky Note");
            GraphEditorData.AddStickyNote(stickyNoteData);

            AddStickyNoteView(stickyNoteData, out HNGraphStickyNoteView stickyNoteView);
        }

        public void AddRelayNode(HNGraphEdgeView edgeView, Vector2 mousePos)
        {
            Undo.RecordObject(GraphEditorData, "Add Relay Node");

            HNGraphConnectionView connectionView = edgeView.ConnectionView;
            HNGraphConnection connectionData = connectionView.ConnectionData;
            HNGraphRelayNode relayNodeData = new HNGraphRelayNode(GraphEditorData, connectionData);
            relayNodeData.Initialize(mousePos);
            GraphEditorData.AddRelayNode(relayNodeData);

            AddRelayNodeView(relayNodeData, out HNGraphRelayNodeView relayNodeView);
            
            connectionView.CreateRelayNodeOnEdge(edgeView, relayNodeView);
        }

        public void DeleteRelayNode(HNGraphRelayNodeView relayNodeView)
        {
            HNGraphPortView relayNodeInputPortView = relayNodeView.InputPortView;
            HNGraphEdgeView inputEdgeView = relayNodeInputPortView.EdgeViews[0];
            HNGraphPortView outputPortView = inputEdgeView.GetAnotherPort(relayNodeInputPortView);
            RemoveElement(inputEdgeView);
            HNGraphPortView relayNodeOutputPortView = relayNodeView.OutputPortView;
            HNGraphEdgeView outputEdgeView = relayNodeOutputPortView.EdgeViews[0];
            HNGraphPortView inputPortView = outputEdgeView.GetAnotherPort(relayNodeOutputPortView);
            RemoveElement(outputEdgeView);
            HNGraphConnectionView connectionView = inputEdgeView.ConnectionView;
            connectionView.RemoveEdgeView(inputEdgeView);
            connectionView.RemoveEdgeView(outputEdgeView);
            connectionView.RemoveRelayNodeView(relayNodeView);
            RemoveElement(relayNodeView);
            HNGraphEdgeView newEdgeView = new HNGraphEdgeView(this);
            newEdgeView.Initialize(connectionView, outputPortView, inputPortView);
            connectionView.AddEdgeView(newEdgeView);
            AddElement(newEdgeView);
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

        private void DrawConnections()
        {
            foreach(var connectionData in GraphEditorData.Connections.Values)
            {
                AddConnectionViewFromData(connectionData);
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

        private void AddConnectionViewFromData(HNGraphConnection connectionData)
        {
            HNGraphPortView output = null;
            HNGraphPortView input = null;
            foreach (var nodeView in nodeViews)
            {
                foreach (var outputPortView in nodeView.OutputPortViews)
                {
                    if (outputPortView.PortData.Guid == connectionData.OutputPort.Guid)
                    {
                        output = outputPortView;
                        break;
                    }
                }
                if(output != null)
                    break;
            }
            foreach (var nodeView in nodeViews)
            {
                foreach (var inputPortView in nodeView.InputPortViews)
                {
                    if (inputPortView.PortData.Guid == connectionData.InputPort.Guid)
                    {
                        input = inputPortView;
                        break;
                    }
                }
                if(input != null)
                    break;
            }
                        
            if (output != null && input != null)
            {
                HNGraphConnectionView connectionView = new HNGraphConnectionView(this, connectionData);
                connectionView.Initialize(output, input);
                connectionViews.Add(connectionView);
                if(connectionData.RelayNodeGuids.Count == 0)
                {
                    HNGraphEdgeView edgeView = new HNGraphEdgeView(this);
                    edgeView.Initialize(connectionView, output, input);
                    connectionView.AddEdgeView(edgeView);
                    AddElement(edgeView);
                }
                else
                {
                    HNGraphPortView lastRelayNodeOutputPortView = output;
                    var relayNodeGuidList = connectionData.RelayNodeGuids;
                    for(int i = 0; i < relayNodeGuidList.Count; i++)
                    {
                        HNGraphRelayNodeView currentRelayNodeView = GetRelayNodeViewFromGuid(relayNodeGuidList[i]);
                        HNGraphEdgeView edgeView = new HNGraphEdgeView(this);
                        edgeView.Initialize(connectionView, lastRelayNodeOutputPortView, currentRelayNodeView.InputPortView);
                        connectionView.AddEdgeView(edgeView);
                        AddElement(edgeView);
                        connectionView.AddRelayNodeView(i, currentRelayNodeView);
                        lastRelayNodeOutputPortView = currentRelayNodeView.OutputPortView;
                    }
                    HNGraphEdgeView lastEdgeView = new HNGraphEdgeView(this);
                    lastEdgeView.Initialize(connectionView, lastRelayNodeOutputPortView, input);
                    connectionView.AddEdgeView(lastEdgeView);
                    AddElement(lastEdgeView);
                }
            }

        }

        private void AddNodeView(HNGraphNode nodeData, out HNGraphNodeView nodeView)
        {
            nodeView = new HNGraphNodeView(this, nodeData, edgeConnectorListener);
            nodeView.Initialize();
            nodeViews.Add(nodeView);
            AddElement(nodeView);
        }

        private void AddConnectionView(HNGraphConnection connectionData, HNGraphEdgeView edgeView, out HNGraphConnectionView connectionView)
        {
            connectionView = new HNGraphConnectionView(this, connectionData);
            connectionView.Initialize(edgeView.OutputPortView, edgeView.InputPortView);
            connectionView.AddEdgeView(edgeView);
            connectionViews.Add(connectionView);
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
            relayNodeViews.Add(relayNodeView);
            AddElement(relayNodeView);
        }

        private void OnConnectionAdded(HNGraphConnectionView connectionView)
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
                        RemoveConnection(edgeViews[i].ConnectionView);
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
                        RemoveConnection(edgeViews[i].ConnectionView);
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
                        if(relayNodeViews[i].InputPortView.ConnectionViews.Count > 0)
                            RemoveConnection(relayNodeViews[i].InputPortView.ConnectionViews[0]);
                    }
                }
            }

            return graphViewChange;
        }

        private void RemoveNode(HNGraphNodeView nodeView)
        {
            foreach(var portView in nodeView.InputPortViews)
            {
                var connectionGuids = portView.PortData.ConnectionGuids;
                foreach(var connectionGuid in connectionGuids)
                {
                    HNGraphConnectionView connectionView = GetConnectionViewFromGuid(connectionGuid);
                    RemoveConnection(connectionView);
                }
            }
            GraphEditorData.RemoveNode(nodeView.NodeData);
            RemoveElement(nodeView);
            nodeViews.Remove(nodeView);
        }

        private void RemoveConnection(HNGraphConnectionView connectionView)
        {
            if(connectionView == null)
                return;

            connectionView.RemoveAllEdgeViews();

            for(int i = relayNodeViews.Count - 1; i >= 0; i--)
            {
                connectionView.RemoveRelayNodeView(relayNodeViews[i]);
                GraphEditorData.RemoveRelayNode(relayNodeViews[i].RelayNodeData);
                RemoveElement(relayNodeViews[i]);
                relayNodeViews.Remove(relayNodeViews[i]);
            }

            GraphEditorData.RemoveConnection(connectionView.ConnectionData);
            connectionViews.Remove(connectionView);
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

                    var edges = inputPort.ConnectionGuids.ToList();
                    foreach(var edge in edges)
                    {
                        HNGraphPort connectPort = GraphEditorData.GetConnection(edge)?.OutputPort;
                        if(connectPort != null && connectPort.PortCapacity != HNGraphPort.Capacity.Single)
                        {
                            HNGraphConnection connection = new HNGraphConnection(GraphEditorData, connectPort, inputPort);
                            connection.Initialize();
                            AddConnectionViewFromData(connection);
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

        private HNGraphConnectionView GetConnectionViewFromGuid(string guid)
        {
            foreach(var connectionView in connectionViews)
            {
                if(guid == connectionView.ConnectionData.Guid)
                    return connectionView;
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
    }
}
