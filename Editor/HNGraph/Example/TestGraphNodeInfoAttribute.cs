using System.Collections;
using System.Collections.Generic;
using HN.Graph.Editor;
using UnityEngine;

namespace HN.Graph.Example.Editor
{
    public class TestGraphNodeInfoAttribute : HNGraphNodeInfoAttribute
    {
        public TestGraphNodeInfoAttribute(string nodeTitle, string menuItem = "") : base(nodeTitle, menuItem)
        {
            
        }


    }
}
