using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HN.Graph.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using Unity.VisualScripting;

namespace HN.Graph.Example.Editor
{
    [ScriptedImporter(1, TestGraph.TestGraphExtension)]
    public class TestGraphImporter : HNGraphImporter<TestGraph>
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            LoadGraphData(ctx.assetPath);
            DeserializeGraphData(ctx);
            SetObject(ctx);
        }
    }


    [CustomEditor(typeof(TestGraphImporter))]
    public class TestGraphImporterEditor : HNGraphImporterEditor
    {

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(new GUIContent("Open Graph")))
            {
                OnOpenButtonClick();
            }

            base.OnInspectorGUI();
        }

        private void OnOpenButtonClick()
        {
            TestGraphImporter importer = target as TestGraphImporter;
            TestGraph graphData = LoadGraphData<TestGraph>();
            OpenGraph<TestGraphEditorWindow, TestGraphEditorData>(importer.assetPath, TestGraph.TestGraphExtension, graphData);
        }


        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string path = AssetDatabase.GetAssetPath(instanceID);
            TestGraph graphData = AssetDatabase.LoadAssetAtPath<TestGraph>(path);
            return OpenGraph<TestGraphEditorWindow, TestGraphEditorData>(path, TestGraph.TestGraphExtension, graphData);
        }

    }
}
