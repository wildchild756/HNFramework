using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphCopyPasteData
    {
        [SerializeField]
        public List<HNGraphNode> serializedNodes = new List<HNGraphNode>();
        
        [SerializeField]
        public List<HNGraphStickyNote> serializedStickyNotes = new List<HNGraphStickyNote>();


    }
}
