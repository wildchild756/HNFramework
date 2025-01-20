using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge;
using Unity.VisualScripting;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphStickyNote : IDisposable, IPositionable
    {
        public string Guid => guid;

        public string Title => title;

        public string Content => content;

        // public HNGraphData EditorData
        // {
        //     set { editorData = value; }
        // }


        [SerializeField]
        private string guid;

        [SerializeField]
        private string title = "Title";

        [SerializeField]
        private string content = "Content";

        [SerializeField]
        private Rect layout;
        
        // private HNGraphData editorData;


        public HNGraphStickyNote(string title)
        {            
            this.title = title;
        }

        public virtual void Initialize(Vector2 newPos)
        {
            guid = HNGraphUtils.NewGuid();
            layout.position = newPos;
            layout.size = new Vector2(400, 200);
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
            return layout;
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
