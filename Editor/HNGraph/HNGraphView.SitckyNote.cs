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
        public void AddStickyNote(Vector2 position)
        {
            GraphEditorData.Owner.RecordObject("Add Sticky Note");

            HNGraphStickyNote stickyNoteData = new HNGraphStickyNote("Create Note");
            stickyNoteData.Initialize(position);
            GraphEditorData.AddStickyNote(stickyNoteData);

            AddStickyNoteView(stickyNoteData, out HNGraphStickyNoteView stickyNoteView);
            
            graphViewElementsCreated(new HNGraphViewCreateElements(stickyNoteView));
        }

        private void DrawStickyNotes()
        {
            foreach(var stickyNoteGuid in GraphEditorData.GetAllStickyNoteGuids())
            {
                HNGraphStickyNote stickyNoteData = GraphEditorData.GetStickyNote(stickyNoteGuid);
                AddStickyNoteView(stickyNoteData, out HNGraphStickyNoteView stickyNoteView);
            }

        }

        private void AddStickyNoteView(HNGraphStickyNote stickyNoteData, out HNGraphStickyNoteView stickyNoteView)
        {
            stickyNoteView = new HNGraphStickyNoteView(this, stickyNoteData);
            stickyNoteView.Initialize();
            AddGraphElement(stickyNoteView);
        }

        private void RemoveStickyNote(HNGraphStickyNoteView stickyNoteView)
        {
            if(!ContainsElement(stickyNoteView))
                return;

            GraphEditorData.RemoveStickyNote(stickyNoteView.StickyNoteData);
            RemoveGraphElement(stickyNoteView);
        }

        private HNGraphStickyNoteView GetStickyNoteViewFromGuid(string guid)
        {
            foreach(var stickyNoteView in stickyNoteViews)
            {
                if(guid == stickyNoteView.StickyNoteData.Guid)
                    return stickyNoteView;
            }
            return null;
        }

    }
}
