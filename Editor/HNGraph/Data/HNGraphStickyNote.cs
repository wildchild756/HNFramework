using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge;
using Unity.VisualScripting;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphStickyNote : IDisposable
    {
        public string Guid => guid;
        [SerializeField]
        private string guid;
        
        public string Title => title;
        [SerializeField]
        private string title = "Title";

        public string Content => content;
        [SerializeField]
        private string content = "Content";

        [SerializeField]
        private Rect layout;

        [SerializeReference]
        private HNGraphEditorData editorData;


        public HNGraphStickyNote(HNGraphEditorData editorData, string title, Vector2 position)
        {
            this.editorData = editorData;
            
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

        public void SetContent(string content)
        {
            this.content = content;
        }

        public Rect GetLayout()
        {
            return this.layout;
        }

        public void SetLayout(Rect layout)
        {
            this.layout = layout;
        }

        public virtual void Dispose()
        {
            
        }
    }


}
