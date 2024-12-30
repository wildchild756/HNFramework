using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Serialize;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using Unity.VisualScripting;
using System.Text;

namespace HN.Graph.Editor
{
    public abstract class HNGraphImporter<T, U> : ScriptedImporter 
    where T : HNGraphData
    where U : HNGraphObject
    {
        protected string iconPath = "";
        protected HNGraphData graphData;
        protected HNGraphObject graphObject;


        public virtual void LoadGraphData(string path)
        {
            graphData = Activator.CreateInstance<T>() as HNGraphData;
            graphData.Initialize(path);
            if(graphData == null)
                return;
            
            graphData.Deserialize();
        }

        public virtual void DeserializeGraphData(AssetImportContext ctx)
        {
            if(graphData == null)
                return;
            
            graphData.GenerateGraphObject<U>();
            graphObject = graphData.GraphObject;
        }

        public virtual void SetObject(AssetImportContext ctx)
        {
            if(graphObject == null)
                return;
            
            if(string.IsNullOrEmpty(iconPath))
                ctx.AddObjectToAsset("MainAsset", graphObject);
            else
            {
                Texture2D texture = Resources.Load<Texture2D>(iconPath);
                if(texture == null)
                    ctx.AddObjectToAsset("MainAsset", graphObject);
                else
                    ctx.AddObjectToAsset("MainAsset", graphObject, texture);
            }
            ctx.SetMainObject(graphObject);
        }
    }
}
