using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace HN.Graph.Editor
{
    public class HNGraphNodeView : Node
    {
        public string Guid => guid;
        private string guid;

        public HNGraphNode NodeData => nodeData;
        private HNGraphNode nodeData;

        public HNGraphEdgeConnectorListener EdgeConnectorListener => edgeConnectorListener;
        private HNGraphEdgeConnectorListener edgeConnectorListener;

        public List<HNGraphPortView> InputPortViews => inputPortViews;
        private List<HNGraphPortView> inputPortViews;

        public List<HNGraphPortView> OutputPortViews => outputPortViews;
        private List<HNGraphPortView> outputPortViews;

        private Type passType;


        public HNGraphNodeView(HNGraphNode nodeData, HNGraphEdgeConnectorListener edgeConnectorListener)
        {
            AddToClassList("graph-node");
            this.edgeConnectorListener = edgeConnectorListener;
            this.nodeData = nodeData;
            this.guid = nodeData.Guid;
            inputPortViews = new List<HNGraphPortView>();
            outputPortViews = new List<HNGraphPortView>();
            passType = nodeData.GraphNodeClass.GetType();

            DrawNode();
            DrawPort();

        }

        public int GetInputPortViewIndex(HNGraphPortView portView)
        {
            if (inputPortViews.Contains(portView))
            {
                return inputPortViews.IndexOf(portView);
            }
            return -1;
        }

        public int GetOutputPortViewIndex(HNGraphPortView portView)
        {
            if (outputPortViews.Contains(portView))
            {
                return outputPortViews.IndexOf(portView);
            }
            return -1;
        }

        private void DrawNode()
        {
            HNGraphNodeInfoAttribute info = passType.GetCustomAttribute<HNGraphNodeInfoAttribute>();
            title = info.NodeTitle;
            name = passType.Name;

            string[] depths = info.MenuItem.Split('/');
            foreach (string depth in depths)
            {
                AddToClassList(depth.ToLower().Replace(' ', '-'));
            }
        }

        private void DrawPort()
        {
            PropertyInfo[] propertiesInfo = passType.GetProperties();
            foreach (var propertyInfo in propertiesInfo)
            {
                HNGraphPortInfoAttribute slotInfo = propertyInfo.GetCustomAttribute<HNGraphPortInfoAttribute>();
                if (slotInfo != null)
                {
                    Direction portDir = (slotInfo.Type == HNGraphPortInfoAttribute.SlotType.Input ? Direction.Input : Direction.Output);
                    HNGraphPortView portView = new HNGraphPortView(portDir, edgeConnectorListener, slotInfo.SlotName);
                    if (slotInfo.Type == HNGraphPortInfoAttribute.SlotType.Input)
                    {
                        inputPortViews.Add(portView);
                        inputContainer.Add(portView);
                    }
                    else
                    {
                        outputPortViews.Add(portView);
                        outputContainer.Add(portView);
                    }
                }
            }
        }

        public void SavePosition()
        {
            nodeData.SetPosition(GetPosition());
        }
    }
}
