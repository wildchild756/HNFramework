using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphNodePort : HNGraphBasePort
    {
        public string PropertyName => propertyName;

        private string propertyName;

        public HNGraphNodePort(string ownerNodeGuid, string typeName, string name, string propertyName, Direction direction, Capacity capacity) : base(ownerNodeGuid, typeName, name, direction, capacity)
        {
            this.propertyName = propertyName;
        }


        public override List<HNGraphNode> GetConnectedNodes(bool isInputPort, HNGraphData editorData)
        {
            List<HNGraphNode> connectedNodes = new List<HNGraphNode>();
            if(editorData == null || edgeGuids.Count == 0)
                return connectedNodes;
            
            for(int i = 0; i < edgeGuids.Count; i++)
            {
                HNGraphEdge edge = editorData.GetEdge(edgeGuids[i]);
                HNGraphBaseNode outputPortOwnerNode = editorData.GetBaseNode(edge.GetOutputPort(editorData).OwnerNodeGuid);
                HNGraphBaseNode inputPortOwnerNode = editorData.GetBaseNode(edge.GetInputPort(editorData).OwnerNodeGuid);
                HNGraphBaseNode node = isInputPort ? outputPortOwnerNode : inputPortOwnerNode;
                if(node == null)
                    continue;

                if(node is HNGraphNode)
                    connectedNodes.Add(node as HNGraphNode);
                else if(node is HNGraphRelayNode)
                {
                    HNGraphRelayNode relayNode = node as HNGraphRelayNode;
                    HNGraphRelayNodePort relayNodeInputPort = relayNode.GetInputPort(editorData);
                    HNGraphRelayNodePort relayNodeOutputPort = relayNode.GetOutputPort(editorData);
                    List<HNGraphNode> nextConnectedNodes = isInputPort ? 
                        relayNodeInputPort.GetConnectedNodes(isInputPort, editorData) :
                        relayNodeOutputPort.GetConnectedNodes(isInputPort, editorData);
                    foreach(var nextConnectedNode in nextConnectedNodes)
                        if(!connectedNodes.Contains(nextConnectedNode))
                            connectedNodes.Add(nextConnectedNode);
                }
            }

            return connectedNodes;
        }
    }
}
