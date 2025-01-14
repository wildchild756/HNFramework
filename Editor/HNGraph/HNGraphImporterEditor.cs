using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Serialize;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public abstract class HNGraphImporterEditor : ScriptedImporterEditor
    {
        public T LoadGraphData<T, U>() where T : HNGraphData where U : HNGraphObject
        {
            var importer = (HNGraphImporter<T, U>)target;

            T graphData = Activator.CreateInstance<T>();
            graphData.Initialize(importer.assetPath);
                        
            return graphData;
        }

        public static bool OpenGraph<T, U>(string path, string targetExtension, HNGraphData graphData) 
            where T : HNGraphEditorWindow 
            where U : HNGraphData
        {
            if(graphData == null)
                return false;

            var extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }
            extension = extension.Substring(1).ToLowerInvariant();
            if (extension != targetExtension)
            {
                return false;
            }

            var guid = AssetDatabase.AssetPathToGUID(path);
            foreach (var w in Resources.FindObjectsOfTypeAll<HNGraphEditorWindow>())
            {
                if (w.Guid == guid)
                {
                    w.Focus();
                    return true;
                }
            }

            T window = HNGraphEditorWindow.ShowWindow<T>();
            window.Focus();
            return window.Initialize(guid, graphData);
        }

        

    }
}
