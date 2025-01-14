using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Serialize;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System;

namespace HN.Graph.Editor
{
    public abstract class HNGraphNewAction<T> : EndNameEditAction where T : new() 
    {
        protected HNGraphData graphData;


        public void CreateGraphData(string pathName)
        {
            graphData = Activator.CreateInstance<T>() as HNGraphData;
            graphData.Initialize(pathName);
        }

        public void LoadAsset(string pathName)
        {
            graphData.GraphObject = AssetDatabase.LoadAssetAtPath<HNGraphObject>(pathName);
        }
    }
}
