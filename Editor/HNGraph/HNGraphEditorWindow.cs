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
    public abstract class HNGraphEditorWindow : EditorWindow
    {
        [SerializeField]
        public HNGraphEditorData graphEditorData;
        public string guid;
        public HNGraphSearchWindowProvider searchWindowProvider;

        private string graphEditorDataPath;
        private string graphEditorDataName;
        private string titleContentString;


        public abstract void OnInitialize();


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

            OnInitialize();

            titleContent = new GUIContent(graphEditorDataName);

            var graphView = new HNGraphView(this, graphEditorData, searchWindowProvider);
            graphView.name = "HNGraphView";
            graphView.graphViewChanged += OnChange;

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
            toolbar.name = "Toolbar";
            graphView.Add(toolbar);

            rootVisualElement.Add(graphView);

            return true;
        }


        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            this.hasUnsavedChanges = true;
            EditorUtility.SetDirty(graphEditorData);
            return graphViewChange;
        }


        public static T ShowWindow<T>() where T : HNGraphEditorWindow
        {
            T window = CreateWindow<T>(typeof(T), typeof(SceneView));
            return window;
        }

    }
}

