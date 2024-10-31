using System.Collections;
using System.Collections.Generic;
using HN.Serialize;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace HN.Graph.Editor
{
    public abstract class HNGraphImporter<T> : ScriptedImporter where T : HNGraphObject
    {
        private T graphData;


        public virtual void LoadGraphData(string path)
        {
            graphData = AssetDatabase.LoadAssetAtPath<T>(path);
            if(graphData == null)
            {
                graphData = ScriptableObject.CreateInstance<T>();
            }
        }

        public virtual void DeserializeGraphData(AssetImportContext ctx)
        {
            Json.Deserialize(graphData, ctx.assetPath);
        }

        public virtual void SetObject(AssetImportContext ctx)
        {
            ctx.AddObjectToAsset("MainAsset", graphData);
            ctx.SetMainObject(graphData);
        }
    }
}
