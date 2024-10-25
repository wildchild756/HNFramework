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
        public static bool OpenGraph<T>(string path, string targetExtension, HNGraphEditorData graphEditorData) where T : HNGraphEditorWindow
        {
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
                if (w.guid == guid)
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
