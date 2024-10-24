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
        public HNGraphObject graphData;


        public override void OnImportAsset(AssetImportContext ctx)
        {
            string path = ctx.assetPath;
            graphData = AssetDatabase.LoadAssetAtPath<T>(path);
            if (graphData == null)
            {
                graphData = ScriptableObject.CreateInstance<T>();
            }
            extension = graphData.Extension;
            string textGraph = File.ReadAllText(path, Encoding.UTF8);
            JsonUtility.FromJsonOverwrite(textGraph, graphData);
            if (graphData != null)
            {
                ctx.AddObjectToAsset("MainAsset", graphData);
                ctx.SetMainObject(graphData);
            }
        }


    }
}
