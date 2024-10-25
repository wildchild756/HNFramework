using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace HN.Graph.Editor
{
    public abstract class HNGraphImporter<T> : ScriptedImporter where T : HNGraphObject
    {
        public string Extension => extension;
        private string extension;

        public T GraphData => graphData;
        private T graphData;


        public void LoadGraphData(string path)
        {
            graphData = AssetDatabase.LoadAssetAtPath<T>(path);
            if(graphData == null)
            {
                graphData = ScriptableObject.CreateInstance<T>();
            }
        }

        public void OverwriteGraphDataByJson(AssetImportContext ctx)
        {
            string textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
            JsonUtility.FromJsonOverwrite(textGraph, graphData);
        }

        public void SetObject(AssetImportContext ctx)
        {
            ctx.AddObjectToAsset("MainAsset", graphData);
            ctx.SetMainObject(graphData);
        }
    }
}
