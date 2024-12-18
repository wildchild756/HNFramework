using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Graph.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace HN.Graph.Example.Editor
{
    public class TestGraphEditorWindow : HNGraphEditorWindow
    {
        public override void CreateSearchWindowProvider()
        {
            SearchWindowProvider = ScriptableObject.CreateInstance<HNGraphSearchWindowProvider>();
            SearchWindowProvider.GraphNodeInfoAttributeType = typeof(TestGraphNodeInfoAttribute);
        }

        public override void AdditionalToolButton(Toolbar toolbar)
        {
            var showInspector = new ToolbarToggle();
            showInspector.text = "Inspector";
            toolbar.Add(showInspector);
        }

    }
}
