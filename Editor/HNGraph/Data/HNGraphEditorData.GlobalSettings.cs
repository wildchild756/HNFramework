using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HN.Serialize;
using UnityEditor;
using UnityEngine;

namespace HN.Graph.Editor
{
    public abstract partial class HNGraphEditorData : ScriptableObject, ISerializable
    {
        public const bool SupportVerticalNodePort = false;
        public const HNGraphBasePort.Capacity inputPortCapacity = HNGraphBasePort.Capacity.Single;
        public const HNGraphBasePort.Capacity outputPortCapacity = HNGraphBasePort.Capacity.Multi;



    }

}


