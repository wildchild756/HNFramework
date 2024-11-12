using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    public interface IPositionable
    {
        public Rect GetLayout();
        public void SetLayout(Rect position);
    }
}
