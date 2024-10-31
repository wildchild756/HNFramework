using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Serialize;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace HN.Graph.Editor
{
    public abstract class HNGraphNewAction<T> : EndNameEditAction where T : HNGraphObject
    {
        public T GraphData => graphData;
        private T graphData;


        public void CreateGraphData()
        {
            graphData = ScriptableObject.CreateInstance<T>();
        }

        public void Serialize(string pathName)
        {
            Json.Serialize(graphData, pathName);
            AssetDatabase.Refresh();
        }
    }
}
