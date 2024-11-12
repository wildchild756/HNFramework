using System.Collections;
using System.Collections.Generic;
using HN.Graph;
using UnityEngine;

namespace HN.Graph.Example
{
    public class TestGraphPortInfoAttribute : HNGraphPortInfoAttribute
    {
        public TestGraphPortInfoAttribute(string slotName, Direction direction, Capacity capacity)
             : base(slotName, direction, capacity)
        {

        }
    }
}
