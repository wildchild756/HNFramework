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
        public void AddGroup(Vector2 position)
        {
            GraphEditorData.Owner.RecordObject("Add Group");

            HNGraphGroup groupData = new HNGraphGroup("New Group");
            groupData.Initialize(position);
            GraphEditorData.AddGroup(groupData);
            
            AddGroupView(groupData, out HNGraphGroupView groupView);
            
            graphViewElementsCreated(new HNGraphViewCreateElements(groupView));
        }

        private void DrawGroups()
        {
            foreach(var groupData in GraphEditorData.Groups.Values)
            {
                AddGroupView(groupData, out HNGraphGroupView groupView);
            }
        }

        private void AddGroupView(HNGraphGroup groupData, out HNGraphGroupView groupView)
        {
            groupView = new HNGraphGroupView(this, groupData);
            groupView.Initialize();
            groupViews.Add(groupView);
            AddElement(groupView);
        }

        private void RemoveGroup(HNGraphGroupView groupView)
        {
            GraphEditorData.RemoveGroup(groupView.GroupData);
            RemoveElement(groupView);
            groupViews.Remove(groupView);
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

        
    }

    
}
