using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public void SaveDataToJson(string pathName)
        {
            string graphDataJson = JsonUtility.ToJson(graphData);
            StreamWriter sw;
            sw = File.CreateText(pathName);
            sw.Write(graphDataJson);
            sw.Close();
            sw.Dispose();
            AssetDatabase.Refresh();
        }
    }
}
