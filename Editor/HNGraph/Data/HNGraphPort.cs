using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    [Serializable]
    public class HNGraphPort : HNGraphBasePort
    {
        public HNGraphPort(HNGraphBaseNode ownerNode, string typeName, string name, Direction direction, Capacity capacity) : base(ownerNode, typeName, name, direction, capacity)
        {
        }


        public override List<HNGraphNode> GetConnectedNodes(bool isInputPort, HNGraphData editorData)
        {
            List<HNGraphNode> connectedNodes = new List<HNGraphNode>();
            if(editorData == null || edgeGuids.Count == 0)
                return connectedNodes;
            
            for(int i = 0; i < edgeGuids.Count; i++)
            {
                HNGraphEdge edge = editorData.Edges[edgeGuids[i]];
                HNGraphBaseNode node = isInputPort ? edge.OutputPort.OwnerNode : edge.InputPort.OwnerNode;
                if(node == null)
                    continue;

                if(node is HNGraphNode)
                    connectedNodes.Add(node as HNGraphNode);
                else if(node is HNGraphRelayNode)
                {
                    HNGraphRelayNode relayNode = node as HNGraphRelayNode;
                    List<HNGraphNode> nextConnectedNodes = 
                        isInputPort ? 
                        relayNode.InputPort.GetConnectedNodes(isInputPort, editorData) :
                        relayNode.OutputPort.GetConnectedNodes(isInputPort, editorData);
                    foreach(var nextConnectedNode in nextConnectedNodes)
                        if(!connectedNodes.Contains(nextConnectedNode))
                            connectedNodes.Add(nextConnectedNode);
                }
            }

            return connectedNodes;
        }
    }
}
