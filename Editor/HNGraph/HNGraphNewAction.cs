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
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            T graphData = ScriptableObject.CreateInstance<T>();
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
