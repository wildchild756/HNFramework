using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    public interface IHNGraphFloatingPanel : IDisposable, IPositionable
    {
        // [SerializeField]
        // protected Rect layout;
        
        // [SerializeReference]
        // protected HNGraphEditorData editorData;


        // public HNGraphFloatingPanel(HNGraphEditorData editorData)
        // {
        //     this.editorData = editorData;
        // }

        public void Initialize();
        // {
        //     layout.position = newPos;
        //     layout.size = new Vector2(300, 400);
        // }

        // public Rect GetLayout()
        // {
        //     return layout;
        // }

        // public void SetLayout(Rect layout)
        // {
        //     this.layout = layout;
        // }

        // public virtual void Dispose()
        // {

        // }
    }
}
