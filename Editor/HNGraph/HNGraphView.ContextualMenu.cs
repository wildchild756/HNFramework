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
            evt.menu.InsertAction(menuPosition, "Create Group", (e) => AddGroup(position), DropdownMenuAction.AlwaysEnabled);
        }

        private void BuildStickNoteContextualMenu(ContextualMenuPopulateEvent evt, int menuPosition = -1)
        {
            if(menuPosition == -1)
                menuPosition = evt.menu.MenuItems().Count;
            
            var target = evt.currentTarget as HNGraphView;
            if(target == null)
                return;

            Vector2 position = target.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            evt.menu.InsertAction(menuPosition, "Create Sticky Note", (e) => AddStickyNote(position), DropdownMenuAction.AlwaysEnabled);
        }
    }
}
