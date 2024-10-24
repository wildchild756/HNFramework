using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public class HNGraphEditorWindow : EditorWindow
    {
        [SerializeField]
        public HNGraphEditorData graphEditorData;
        public string guid;

        private string graphEditorDataPath;
        private string graphEditorDataName;
        private string titleContentString;

        public bool Initialize(string assetGuid, HNGraphEditorData graphEditorData)
        {
            guid = assetGuid;
            graphEditorDataPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            graphEditorDataName = Path.GetFileNameWithoutExtension(graphEditorDataPath);
            if (graphEditorData == null)
            {
                return false;
            }
            this.graphEditorData = graphEditorData;
            titleContentString = graphEditorDataName;

            titleContent = new GUIContent(graphEditorDataName);

            var graphView = new HNGraphView(this, graphEditorData);
            rootVisualElement.Add(graphView);

            var toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                {
                    Debug.Log("Save Asset");
                }
                GUILayout.Space(6);
                if (GUILayout.Button("Save As...", EditorStyles.toolbarButton))
                {
                    Debug.Log("Save As...");
                }
                GUILayout.Space(6);
                if (GUILayout.Button("Discard Changes", EditorStyles.toolbarButton))
                {
                    Debug.Log("Discard Changes");
                }
                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                    Debug.Log("Show in Project");
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            });
            //rootVisualElement.Add(toolbar);

            graphView.graphViewChanged += OnChange;

            return true;
        }


        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            this.hasUnsavedChanges = true;
            EditorUtility.SetDirty(graphEditorData);
            return graphViewChange;
        }


        public static HNGraphEditorWindow ShowWindow()
        {
            HNGraphEditorWindow window = CreateWindow<HNGraphEditorWindow>(typeof(HNGraphEditorWindow), typeof(SceneView));
            return window;
        }

    }
}

