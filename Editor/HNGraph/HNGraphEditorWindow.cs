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
        public string Guid => guid;
        private string guid;

        public HNGraphSearchWindowProvider SearchWindowProvider
        {
            get { return searchWindowProvider; }
            set { searchWindowProvider = value; }
        }
        private HNGraphSearchWindowProvider searchWindowProvider;

        private HNGraphEditorData graphEditorData;
        private HNGraphView graphView;
        private IMGUIContainer toolbar;
        private string graphEditorDataPath;
        private string graphEditorDataName;
        private string extension;


        public abstract void OnInitialize();


        public bool Initialize(string assetGuid, HNGraphEditorData graphEditorData)
        {
            guid = assetGuid;
            graphEditorDataPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            graphEditorDataName = Path.GetFileNameWithoutExtension(graphEditorDataPath);
            titleContent = new GUIContent($"{graphEditorDataName}({graphEditorData.graphData.GetType().Name})");
            extension = Path.GetExtension(graphEditorDataPath).Split(".")[1];
            if (graphEditorData == null)
            {
                return false;
            }
            this.graphEditorData = graphEditorData;

            OnInitialize();

            if(searchWindowProvider == null)
            {
                return false;
            }

            graphView = new HNGraphView(this, graphEditorData, searchWindowProvider);
            graphView.name = "HNGraphView";
            graphView.graphViewChanged += OnChange;

            toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                {
                    OnSaveAsset();
                }
                GUILayout.Space(6);
                if (GUILayout.Button("Save As...", EditorStyles.toolbarButton))
                {
                    OnSaveAs();
                }
                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                    OnShowInProject();
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

        public override void SaveChanges()
        {
            base.SaveChanges();

            graphEditorData.SaveAsset();
        }


        public static T ShowWindow<T>() where T : HNGraphEditorWindow
        {
            T window = CreateWindow<T>(typeof(T), typeof(SceneView));
            return window;
        }


        private void OnSaveAsset()
        {
            graphEditorData.SaveAsset();
            hasUnsavedChanges = false;
        }


        private void OnSaveAs()
        {
            if(string.IsNullOrEmpty(graphEditorDataPath) || string.IsNullOrEmpty(extension))
            {
                return;
            }

            string oldDirectory = Path.GetDirectoryName(graphEditorDataPath);
            string newPath = EditorUtility.SaveFilePanelInProject("Save Graph As...", graphEditorDataName, extension, "", oldDirectory);
            newPath = newPath.Replace(Application.dataPath, "Assets");

            if(!string.IsNullOrEmpty(newPath))
            {
                if (newPath != graphEditorDataPath)
                {
                    Type type = graphEditorData.graphData.GetType();
                    HNGraphObject newGraphData = (HNGraphObject)ScriptableObject.CreateInstance(type);
                    if (newGraphData != null)
                    {
                        newGraphData.AssetPath = newPath;
                        graphEditorData.graphData = newGraphData;
                    }
                    graphEditorData.SaveAsset();
                    AssetDatabase.ImportAsset(newPath);

                    newGraphData = (HNGraphObject)AssetDatabase.LoadAssetAtPath(newPath, type);
                    graphEditorData.graphData = newGraphData;
                    guid = AssetDatabase.AssetPathToGUID(newPath);
                    graphEditorDataPath = newPath;
                    graphEditorDataName = Path.GetFileNameWithoutExtension(graphEditorDataPath);
                    titleContent = new GUIContent(($"{graphEditorDataName}({graphEditorData.graphData.GetType().Name})"));
                }
            }
        }
        
        private void OnShowInProject()
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(graphEditorDataPath);
            EditorGUIUtility.PingObject(asset);
        }

    }
}

