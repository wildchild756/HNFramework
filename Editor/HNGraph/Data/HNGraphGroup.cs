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
        [SerializeField]
        private string guid;

        public string Title => title;
        [SerializeField]
        private string title;

        [SerializeField]
        private Rect layout;

        public List<string> InnerNodeGuids => InnerNodeGuids;
        [SerializeField]
        private List<string> innerNodeGuids;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphGroup(HNGraphEditorData editorData, string title, Vector2 position)
        {
            this.editorData = editorData;
            
            innerNodeGuids = new List<string>();
            this.title = title;
            this.layout.position = position;

            OnCreate(position);
        }

        public virtual void OnCreate(Vector2 newPos)
        {
            guid = HNGraphUtils.NewGuid();
            SetLayout(new Rect(newPos, new Vector2(400, 200)));
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public Rect GetLayout()
        {
            return this.layout;
        }

        public void SetLayout(Rect layout)
        {
            this.layout = layout;
        }

        public bool ContainsNode(string nodeGuid)
        {
            return innerNodeGuids.Contains(nodeGuid);
        }

        public void AddNode(string nodeGuid)
        {
            if(!innerNodeGuids.Contains(nodeGuid))
            {
                innerNodeGuids.Add(nodeGuid);
            }
        }

        public void RemoveNode(string nodeGuid)
        {
            if(innerNodeGuids.Contains(nodeGuid))
            {
                innerNodeGuids.Remove(nodeGuid);
            }
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
