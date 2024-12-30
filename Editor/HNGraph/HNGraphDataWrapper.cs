using System;
using System.Collections;
using System.Collections.Generic;
using HN.Editor;
using UnityEngine;

namespace HN.Graph.Editor
{
    public class HNGraphDataWrapper : HNUndoableObject
    {
        public HNGraphData GraphData => graphData;


        [SerializeField]
        private HNGraphData graphData;

        
        public void Initialize(HNGraphData graphData)
        {
            this.graphData = graphData;
            graphData.Owner = this;
        }
        
    }
}
