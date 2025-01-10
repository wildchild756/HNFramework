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

        private void RemoveFloatingPanelView(HNGraphFloatingPanelView floatingPanelView)
        {
            RemoveGraphElement(floatingPanelView);
            floatingPanelView.Dispose();
        }

    }
}
