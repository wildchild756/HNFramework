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

        public HNGraphSearchWindowProvider SearchWindowProvider
        {
            get { return searchWindowProvider; }
            set { searchWindowProvider = value; }
        }

        public HNGraphView GraphView => graphView;

        public HNGraphData GraphData => graphData;


        private string guid;
        private HNGraphSearchWindowProvider searchWindowProvider;

        protected HNGraphData graphData;
        private HNGraphView graphView;
        private IMGUIContainer toolbar;
        protected string graphDataPath;
        private string graphDataName;
        private string extension;


        public abstract void CreateSearchWindowProvider();
        public abstract void AdditionalToolButton(Toolbar toolbar);
        protected abstract bool LoadGraphData(string path);


        public bool Initialize(string assetGuid, HNGraphData graphData)
        {
            guid = assetGuid;
            graphDataPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            graphDataName = Path.GetFileNameWithoutExtension(graphDataPath);
            titleContent = new GUIContent($"{graphDataName}({graphData.GraphObject.GetType().Name})");
            extension = Path.GetExtension(graphDataPath).Split(".")[1];
            if (graphData == null)
            {
                return false;
            }
            this.graphData = graphData;

            CreateSearchWindowProvider();

            if(searchWindowProvider == null)
            {
                return false;
            }

            graphView = new HNGraphView(this, graphData, searchWindowProvider);
            graphView.Initialize();
            graphView.name = "GraphView";
            graphView.graphViewChanged += OnChange;
            graphView.graphViewElementsCreated += OnElementsCreate;

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
            
            AdditionalToolButton(toolbar);
            
            return toolbar;
        }

        public override void SaveChanges()
        {
            base.SaveChanges();

            graphData.SaveAsset();
        }


        public static T ShowWindow<T>() where T : HNGraphEditorWindow
        {
            T window = CreateWindow<T>(typeof(T), typeof(SceneView));
            return window;
        }

        void Update()
        {
            if(File.Exists(graphDataPath))
            {
                if(graphData == null || graphView == null)
                {
                    if(LoadGraphData(graphDataPath))
                    {
                        Initialize(AssetDatabase.AssetPathToGUID(graphDataPath), graphData);
                    }
                }
            }
            else
            {
                if(graphView != null)
                {
                    rootVisualElement.Remove(graphView);
                    graphView = null;
                }
            }
        }

        void OnDestroy()
        {
            if(graphData != null && graphData.Owner != null)
                Undo.ClearUndo(graphData.Owner);
        }


        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            SetWindowDirty();
            return graphViewChange;
        }

        private HNGraphViewCreateElements OnElementsCreate(HNGraphViewCreateElements createElements)
        {
            SetWindowDirty();
            return createElements;
        }

        private void SetWindowDirty()
        {
            this.hasUnsavedChanges = true;
            graphData.Dirty = true;
        }

        private void OnSaveAsset()
        {
            graphData.SaveAsset();
            hasUnsavedChanges = false;
        }


        private void OnSaveAs()
        {
            if(string.IsNullOrEmpty(graphDataPath) || string.IsNullOrEmpty(extension))
            {
                return;
            }

            string oldDirectory = Path.GetDirectoryName(graphDataPath);
            string newPath = EditorUtility.SaveFilePanelInProject("Save Graph As...", graphDataName, extension, "", oldDirectory);
            newPath = newPath.Replace(Application.dataPath, "Assets");

            if(!string.IsNullOrEmpty(newPath))
            {
                if (newPath != graphDataPath)
                {
                    Type type = graphData.GraphObject.GetType();
                    HNGraphObject newGraphData = (HNGraphObject)ScriptableObject.CreateInstance(type);
                    if (newGraphData != null)
                    {
                        newGraphData.AssetPath = newPath;
                        graphData.GraphObject = newGraphData;
                    }
                    graphData.SaveAsset();
                    AssetDatabase.ImportAsset(newPath);

                    newGraphData = (HNGraphObject)AssetDatabase.LoadAssetAtPath(newPath, type);
                    graphData.GraphObject = newGraphData;
                    guid = AssetDatabase.AssetPathToGUID(newPath);
                    graphDataPath = newPath;
                    graphDataName = Path.GetFileNameWithoutExtension(graphDataPath);
                    titleContent = new GUIContent(($"{graphDataName}({graphData.GraphObject.GetType().Name})"));
                }
            }
        }
        
        private void OnShowInProject()
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(graphDataPath);
            EditorGUIUtility.PingObject(asset);
        }

    }
}

