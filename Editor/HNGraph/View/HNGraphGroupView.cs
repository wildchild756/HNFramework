using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphGroupView : Group
    {
        public HNGraphGroup GroupData => groupData;
        private HNGraphGroup groupData;


        public HNGraphGroupView(HNGraphGroup groupData)
        {
            this.groupData = groupData;

            title = groupData.Title;
            headerContainer.Q<TextField>().RegisterCallback<ChangeEvent<string>>((e) =>
            {
                groupData.SetTitle(e.newValue);
            });
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            foreach(var element in elements)
            {
                var nodeView = element as HNGraphNodeView;
                if(nodeView == null)
                {
                    continue;
                }

                groupData.AddNode(nodeView.BaseNodeData.Guid);
            }

            base.OnElementsAdded(elements);
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            if(parent == null)
            {
                return;
            }

            foreach( var element in elements)
            {
                if(element is HNGraphNodeView nodeView)
                {
                    groupData.RemoveNode(nodeView.BaseNodeData.Guid);
                }
            }

            base.OnElementsRemoved(elements);
        }

        public void SavePosition()
        {
            groupData.SetLayout(GetPosition());
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            SavePosition();
        }
    }
}
