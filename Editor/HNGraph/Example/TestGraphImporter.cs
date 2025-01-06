using System;
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
    public class TestGraphImporter : HNGraphImporter<TestGraphData, TestGraph>
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
            TestGraphData graphData = LoadGraphData<TestGraphData, TestGraph>();
            OpenGraph<TestGraphEditorWindow, TestGraphData>(importer.assetPath, TestGraph.TestGraphExtension, graphData);
        }


        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string path = AssetDatabase.GetAssetPath(instanceID);
            if(Path.GetExtension(path) != TestGraph.TestGraphExtension)
                return false;
            TestGraphData graphData = Activator.CreateInstance<TestGraphData>();
            graphData.Initialize(path);
            graphData.Deserialize();
            return OpenGraph<TestGraphEditorWindow, TestGraphData>(path, TestGraph.TestGraphExtension, graphData);
        }

    }
}
