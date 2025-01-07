using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    public interface IHNGraphFloatingPanel : IDisposable, IPositionable
    {
        public void Initialize();
    }
}
