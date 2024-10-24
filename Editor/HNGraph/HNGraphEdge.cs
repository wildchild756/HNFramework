using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphEdge
    {
        public string Guid => guid;
        [SerializeField]
        private string guid;

        public string OutputGuid => outputGuid;
        [SerializeField]
        private string outputGuid;

        public string InputGuid => inputGuid;
        [SerializeField]
        private string inputGuid;

        public int OutputPortIndex => outputPortIndex;
        [SerializeField]
        private int outputPortIndex;

        public int InputPortIndex => inputPortIndex;
        [SerializeField]
        private int inputPortIndex;


        public HNGraphEdge(HNGraphPortView outputPortView, HNGraphPortView inputPortView)
        {
            NewGUID();

            HNGraphNodeView outputNodeView = (HNGraphNodeView)outputPortView.node;
            outputGuid = outputNodeView.NodeData.Guid;
            outputPortIndex = outputNodeView.GetOutputPortViewIndex(outputPortView);
            HNGraphNodeView inputNodeView = (HNGraphNodeView)inputPortView.node;
            inputGuid = inputNodeView.NodeData.Guid;
            inputPortIndex = inputNodeView.GetInputPortViewIndex(inputPortView);

        }

        private void NewGUID()
        {
            guid = System.Guid.NewGuid().ToString();
        }
    }
}
