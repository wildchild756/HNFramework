using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HN.Graph.Editor
{
    public struct HNGraphViewCreateElements
    {
        public List<System.Object> createdElements;


        public HNGraphViewCreateElements(System.Object newElement)
        {
            createdElements = new List<System.Object>();
            createdElements.Add(newElement);
        }

        public HNGraphViewCreateElements(System.Object newElement0, System.Object newElement1)
        {
            createdElements = new List<System.Object>();
            createdElements.Add(newElement0);
            createdElements.Add(newElement1);
        }

        public HNGraphViewCreateElements(System.Object newElement0, System.Object newElement1, System.Object newElement2)
        {
            createdElements = new List<System.Object>();
            createdElements.Add(newElement0);
            createdElements.Add(newElement1);
            createdElements.Add(newElement2);
        }

        public HNGraphViewCreateElements(List<System.Object> newElements)
        {
            createdElements = newElements;
        }
    }
}
