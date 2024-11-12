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


        public HNGraphStickyNoteView(HNGraphStickyNote stickyNoteData)
        {
            this.stickyNoteData = stickyNoteData;
            
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

        public void SavePosition()
        {
            var p = GetPosition();
            stickyNoteData.SetLayout(p);
        }

        public override void OnResized()
        {
            stickyNoteData.SetLayout(layout);
        }
    }
}
