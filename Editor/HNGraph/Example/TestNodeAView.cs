using System.Collections;
using System.Collections.Generic;
using HN.Graph.Editor;
using UnityEngine;

namespace HN.Graph.Example.Editor
{
    [TestGraphNodeInfo("Test Node A", "Test Catgory A/Test Node A")]
    public class TestNodeA : TestGraphNode
    {
        [TestGraphPortInfo("Test Int Input Port Single", HNGraphPortInfo.Orientation.Horizontal, HNGraphPortInfo.Direction.Input, HNGraphPortInfo.Capacity.Single)]
        public int TestIntInputPortSingle => testIntInputPortSingle;
        private int testIntInputPortSingle;

        [TestGraphPortInfo("Test Int Input Port Multi", HNGraphPortInfo.Orientation.Vertical, HNGraphPortInfo.Direction.Input, HNGraphPortInfo.Capacity.Multi)]
        public int TestIntInputPortMulti => testIntInputPortMulti;
        private int testIntInputPortMulti;

        [TestGraphPortInfo("Test Int Output Port Single", HNGraphPortInfo.Orientation.Horizontal, HNGraphPortInfo.Direction.Output, HNGraphPortInfo.Capacity.Single)]
        public int TestIntOutputPortSingle => testIntOutputPortSingle;
        private int testIntOutputPortSingle;

        [TestGraphPortInfo("Test Int Output Port Multi", HNGraphPortInfo.Orientation.Vertical, HNGraphPortInfo.Direction.Output, HNGraphPortInfo.Capacity.Multi)]
        public int TestIntOutputPortMulti => testIntOutputPortMulti;
        private int testIntOutputPortMulti;
    }
}
