using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphStickyNoteView : StickyNote
    {
        public HNGraphStickyNote StickyNoteData => stickyNoteData;

        
        private HNGraphStickyNote stickyNoteData;

        private HNGraphView graphView;


        public HNGraphStickyNoteView(HNGraphView graphView, HNGraphStickyNote stickyNoteData)
        {
            this.graphView = graphView;
            this.stickyNoteData = stickyNoteData;
            this.stickyNoteData.EditorData = graphView.GraphEditorData;
            
            this.Q<TextField>("title-field").RegisterCallback<ChangeEvent<string>>(e => 
            {
                stickyNoteData.SetTitle(e.newValue);
            });
            this.Q<TextField>("contents-field").RegisterCallback<ChangeEvent<string>>(e => 
            {
                stickyNoteData.SetContent(e.newValue);
            });

            title = stickyNoteData.Title;
            contents = stickyNoteData.Content;
        }

        public void Initialize()
        {
            SetPosition(stickyNoteData.GetLayout());
        }

        public void SavePosition()
        {
            stickyNoteData.SetLayout(GetPosition());
        }

        public override void OnResized()
        {
            stickyNoteData.SetLayout(layout);
        }
    }
}
