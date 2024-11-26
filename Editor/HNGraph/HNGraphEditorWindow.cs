using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

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


        public abstract void CreateSearchWindowProvider();
        public abstract void AdditionalToolButon(Toolbar toolbar);


        public bool Initialize(string assetGuid, HNGraphEditorData graphEditorData)
        {
            guid = assetGuid;
            graphEditorDataPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            graphEditorDataName = Path.GetFileNameWithoutExtension(graphEditorDataPath);
            titleContent = new GUIContent($"{graphEditorDataName}({graphEditorData.GraphData.GetType().Name})");
            extension = Path.GetExtension(graphEditorDataPath).Split(".")[1];
            if (graphEditorData == null)
            {
                return false;
            }
            this.graphEditorData = graphEditorData;

            CreateSearchWindowProvider();

            if(searchWindowProvider == null)
            {
                return false;
            }

            graphView = new HNGraphView(this, graphEditorData, searchWindowProvider);
            graphView.name = "GraphView";
            graphView.graphViewChanged += OnChange;

            var toolbar = GetToolbar();
            toolbar.name = "Toolbar";
            graphView.Add(toolbar);

            rootVisualElement.Add(graphView);

            return true;
        }


        private Toolbar GetToolbar()
        {
            Toolbar toolbar = new Toolbar();
            toolbar.name = "Toolbar";
            var saveButton = new ToolbarButton(OnSaveAsset);
            saveButton.text = "Save Asset";
            toolbar.Add(saveButton);
            var saveAsButton = new ToolbarButton(OnSaveAs);
            saveAsButton.text = "Save As...";
            toolbar.Add(saveAsButton);
            var showInProjectButton = new ToolbarButton(OnShowInProject);
            showInProjectButton.text = "Show In Project";
            toolbar.Add(showInProjectButton);

            var spacer = new ToolbarSpacer();
            toolbar.Add(spacer);
            
            AdditionalToolButon(toolbar);
            
            return toolbar;
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
                    Type type = graphEditorData.GraphData.GetType();
                    HNGraphObject newGraphData = (HNGraphObject)ScriptableObject.CreateInstance(type);
                    if (newGraphData != null)
                    {
                        newGraphData.AssetPath = newPath;
                        graphEditorData.GraphData = newGraphData;
                    }
                    graphEditorData.SaveAsset();
                    AssetDatabase.ImportAsset(newPath);

                    newGraphData = (HNGraphObject)AssetDatabase.LoadAssetAtPath(newPath, type);
                    graphEditorData.GraphData = newGraphData;
                    guid = AssetDatabase.AssetPathToGUID(newPath);
                    graphEditorDataPath = newPath;
                    graphEditorDataName = Path.GetFileNameWithoutExtension(graphEditorDataPath);
                    titleContent = new GUIContent(($"{graphEditorDataName}({graphEditorData.GraphData.GetType().Name})"));
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

