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
    public abstract class HNGraphImporterEditor<T> : ScriptedImporterEditor where T : HNGraphObject
    {
        private HNGraphImporter<T> importer;


        public override void OnInspectorGUI()
        {
            Debug.Log("123123");
            if (GUILayout.Button(new GUIContent("Open Graph")))
            {
                OnOpenButtonClick();
            }

            ApplyRevertGUI();
        }

        private void OnOpenButtonClick()
        {
            importer = target as HNGraphImporter<T>;
            HNGraphEditorData graphEditorData = ScriptableObject.CreateInstance<HNGraphEditorData>();
            graphEditorData.Initialize(importer.graphData);
            OnOpenGraph(importer.assetPath, importer.Extension, graphEditorData);
        }



        public static bool OnOpenGraph(string path, string targetExtension, HNGraphEditorData graphEditorData)
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

            var window = HNGraphEditorWindow.ShowWindow();
            window.Focus();
            return window.Initialize(guid, graphEditorData);
        }
    }
}
