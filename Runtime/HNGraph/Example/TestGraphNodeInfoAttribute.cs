using System.Collections;
using System.Collections.Generic;
using HN.Graph;
using UnityEngine;

namespace HN.Graph.Example
{
    public class TestGraphNodeInfoAttribute : HNGraphNodeInfoAttribute
    {
        public TestGraphNodeInfoAttribute(string nodeTitle, string menuItem = "") : base(nodeTitle, menuItem)
        {
            
        }


    }
}
