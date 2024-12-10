using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphGroup : IDisposable
    {
        public string Guid => guid;

        public string Title => title;

        public IReadOnlyList<string> InnerNodeGuids => innerNodeGuids;


        [SerializeField]
        private string guid;

        [SerializeField]
        private string title;

        [SerializeField]
        private Rect layout;

        [SerializeField]
        private List<string> innerNodeGuids;
        
        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphGroup(HNGraphEditorData editorData, string title)
        {
            this.editorData = editorData;
            
            innerNodeGuids = new List<string>();
            this.title = title;
        }

        public virtual void Initialize(Vector2 position)
        {
            guid = HNGraphUtils.NewGuid();
            layout.position = position;
            layout.size = new Vector2(400, 200);
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public Rect GetLayout()
        {
            return layout;
        }

        public void SetLayout(Rect layout)
        {
            this.layout = layout;
        }

        public void AddNode(string nodeGuid)
        {
            if(innerNodeGuids.Contains(nodeGuid))
                return;

            innerNodeGuids.Add(nodeGuid);
        }

        public void RemoveNode(string nodeGuid)
        {
            if(!innerNodeGuids.Contains(nodeGuid))
                return;
                
            innerNodeGuids.Remove(nodeGuid);
        }

        public virtual void Dispose()
        {
            foreach(var nodeGuid in innerNodeGuids)
            {
                HNGraphNode node = editorData.GetNode(nodeGuid);
                node.Dispose();
            }
        }
    }
}
