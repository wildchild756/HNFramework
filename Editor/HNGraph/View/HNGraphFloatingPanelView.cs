using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public abstract class HNGraphFloatingPanelView : GraphElement, IDisposable
    {
        private static readonly string floatingPanelTree = "Elements/FloatingPanel";
        private static readonly string floatingPanelStyle = "Elements/FloatingPanel";


        public IHNGraphFloatingPanel FloatingPanelData => floatingPanelData;

        public string Title => title;


        protected IHNGraphFloatingPanel floatingPanelData;

        protected HNGraphView graphView;

        protected VisualElement main;
        protected VisualElement root;
        protected ScrollView scrollView;


        private event Action OnResized;


        public HNGraphFloatingPanelView(HNGraphView graphView, IHNGraphFloatingPanel floatingPanelData)
        {
            this.graphView = graphView;
            this.floatingPanelData = floatingPanelData;

            this.AddManipulator(new Dragger{ clampToParentEdges = true });

            capabilities |= Capabilities.Resizable | Capabilities.Movable;

            var tpl = Resources.Load<VisualTreeAsset>(floatingPanelTree);
            styleSheets.Add(Resources.Load<StyleSheet>(floatingPanelStyle));
            main = tpl.CloneTree();
            main.AddToClassList("mainContainer");
            root = main.Q("Root");
            scrollView = main.Q<ScrollView>("ScrollView");
            Label label = new Label("test label");
            scrollView.Add(label);

            ClearClassList();
            AddToClassList("floatingPanel");
            
            hierarchy.Add(main);
            hierarchy.Add(new Resizer(() => OnResized?.Invoke()));
            RegisterCallback<DragUpdatedEvent>(e =>
            {
                e.StopPropagation();
            });
        }

        public virtual void Initialize()
        {
            SetPosition(floatingPanelData.GetLayout());
            
            OnResized += () =>
            {
                SavePosition();
                SetPosition(floatingPanelData.GetLayout());
                EditorUtility.SetDirty(graphView.GraphEditorWindow);
            };

            RegisterCallback<MouseUpEvent>(e => 
            {
                SavePosition();
                SetPosition(floatingPanelData.GetLayout());
                EditorUtility.SetDirty(graphView.GraphEditorWindow);
            });
        }

        public new void SetPosition(Rect layout)
        {
            base.SetPosition(layout);
        }

        public void SavePosition()
        {
            floatingPanelData.SetLayout(GetPosition());
        }

        public virtual void Dispose()
        {
            
        }
    }
}
