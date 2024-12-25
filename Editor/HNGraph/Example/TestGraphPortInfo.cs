using System.Collections;
using System.Collections.Generic;
using HN.Graph.Editor;
using UnityEngine;

namespace HN.Graph.Example.Editor
{
    public class TestGraphPortInfo : HNGraphPortInfo
    {
        public TestGraphPortInfo(string slotName, Orientation orientation, Direction direction, Capacity capacity)
             : base(slotName, orientation, direction, capacity)
        {

        }
    }
}
