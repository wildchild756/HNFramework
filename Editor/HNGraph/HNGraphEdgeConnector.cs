using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace HN.Graph.Editor
{
    public class HNGraphEdgeConnector : EdgeConnector
    {
        readonly HNGraphEdgeDragHelper m_EdgeDragHelper;
        private HNGraphView graphView;
        Edge m_EdgeCandidate;
        private bool m_Active;
        Vector2 m_MouseDownPosition;

        internal const float k_ConnectionDistanceTreshold = 10f;

        public HNGraphEdgeConnector(HNGraphView graphView, IEdgeConnectorListener listener)
        {
            this.graphView = graphView;
            m_EdgeDragHelper = new HNGraphEdgeDragHelper(listener);
            m_Active = false;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        public override EdgeDragHelper edgeDragHelper => m_EdgeDragHelper;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            target.RegisterCallback<MouseCaptureOutEvent>(OnCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }

        protected virtual void OnMouseDown(MouseDownEvent e)
        {
            if (m_Active)
            {
                e.StopImmediatePropagation();
                return;
            }

            if (!CanStartManipulation(e))
            {
                return;
            }

            var graphElement = target as Port;
            if (graphElement == null)
            {
                return;
            }

            m_MouseDownPosition = e.localMousePosition;
            
            m_EdgeCandidate = new Edge();
            m_EdgeDragHelper.draggedPort = graphElement;
            m_EdgeDragHelper.edgeCandidate = m_EdgeCandidate;
            
            if (m_EdgeDragHelper.HandleMouseDown(e))
            {
                m_Active = true;
                target.CaptureMouse();

                e.StopPropagation();
            }
            else
            {
                m_EdgeDragHelper.Reset();
                m_EdgeCandidate = null;
            }
        }

        void OnCaptureOut(MouseCaptureOutEvent e)
        {
            m_Active = false;
            if (m_EdgeCandidate != null)
                Abort();
        }

        protected virtual void OnMouseMove(MouseMoveEvent e)
        {
            if (!m_Active) return;
            
            m_EdgeDragHelper.HandleMouseMove(e);
            m_EdgeCandidate.candidatePosition = e.mousePosition;
            m_EdgeCandidate.UpdateEdgeControl();
            e.StopPropagation();
        }

        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (!m_Active || !CanStopManipulation(e))
                return;

            if (CanPerformConnection(e.localMousePosition))
                m_EdgeDragHelper.HandleMouseUp(e);
            else
                Abort();

            m_Active = false;
            m_EdgeCandidate = null;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        private void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode != KeyCode.Escape || !m_Active)
                return;

            Abort();

            m_Active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        protected virtual void Abort()
        {
            var graphView = target?.GetFirstAncestorOfType<GraphView>();
            graphView?.RemoveElement(m_EdgeCandidate);

            m_EdgeCandidate.input = null;
            m_EdgeCandidate.output = null;
            m_EdgeCandidate = null;

            m_EdgeDragHelper.Reset();
        }

        bool CanPerformConnection(Vector2 mousePosition)
        {
            return Vector2.Distance(m_MouseDownPosition, mousePosition) > k_ConnectionDistanceTreshold;
        }
    }
}
