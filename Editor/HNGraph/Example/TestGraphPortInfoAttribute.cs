using System.Collections;
using System.Collections.Generic;
using HN.Graph.Editor;
using UnityEngine;

namespace HN.Graph.Example.Editor
{
    public class TestGraphPortInfoAttribute : HNGraphPortInfoAttribute
    {
        public TestGraphPortInfoAttribute(string slotName, Orientation orientation, Direction direction, Capacity capacity)
             : base(slotName, orientation, direction, capacity)
        {

        }
    }
}
