using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Graph.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace HN.Graph.Example.Editor
{
    public class TestGraphData : HNGraphData
    {
        // public override void UpdateGraphObject(ref HNGraphObject graphObject)
        // {
        //     throw new System.NotImplementedException();
        // }

        public override void SaveAsset()
        {
            base.SaveAsset();
        }

    }


    public class TestGraphNewAction : HNGraphNewAction<TestGraph>
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            // CreateGraphData(pathName);
            // Serialize();
            // LoadAsset(pathName);
        }


        [MenuItem("Assets/Create/HN Unity Framework/Test Graph")]
        public static void CreateRenderGraph()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<TestGraphNewAction>(),
                string.Format("New Test Graph.{0}", TestGraph.TestGraphExtension),
                null,
                null);
        }

    }
}
