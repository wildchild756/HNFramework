using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public abstract class HNGraphImporterEditor : ScriptedImporterEditor
    {
        public T LoadGraphData<T>() where T : HNGraphObject
        {
            var importer = (HNGraphImporter<T>)target;
            T graphData = AssetDatabase.LoadAssetAtPath<T>(importer.assetPath);
            graphData.AssetPath = importer.assetPath;
            return graphData;
        }

        public static bool OpenGraph<T, U>(string path, string targetExtension, HNGraphObject graphData) 
            where T : HNGraphEditorWindow 
            where U : HNGraphEditorData
        {
            if(graphData == null)
            {
                return false;
            }

            U graphEditorData = ScriptableObject.CreateInstance<U>();
            JsonUtility.FromJsonOverwrite(graphData.EditorDataJson, graphEditorData);
            graphEditorData.Initialize(graphData);
            
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
            return window.Initialize(guid, graphEditorData);
        }

        

    }
}
