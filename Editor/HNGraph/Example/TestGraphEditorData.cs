using System.Collections;
using System.Collections.Generic;
using System.IO;
using HN.Graph.Example;
using HN.Graph.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace HN.Graph.Editor.Example
{
    public class TestGraphEditorData : HNGraphEditorData
    {
        public override void SaveAsset()
        {
            graphData.SerializedEditorData = Serialize();
            Compile();
            graphData.Serialize();
        }

        public override void Compile()
        {
            TestGraph graph = graphData as TestGraph;
            // if(graph == null || graph.RenderStack == null)
            // {
            //     return;
            // }

            // List<TestOutput> outputNodes = FindNodesWithType<TestOutput>();
            // Debug.Log(outputNodes.Count);
            // foreach(var n in outputNodes)
            // {
            //     Debug.Log(n);
            // }
        }


        // private List<Type> FindNodesWithType<Type>() where Type : TestGraphNode
        // {
        //     List<Type> list = new List<Type>();

        //     var nodes = GetNodesEnumerator();
        //     while(nodes.MoveNext())
        //     {
        //         HNGraphNode node = nodes.Current as HNGraphNode;
        //         if(node == null)
        //         {
        //             continue;
        //         }

        //         if (node.GraphNodeClass != null)
        //         {
        //             Type thisNode = node.GraphNodeClass as Type;
        //             if (thisNode != null)
        //             {
        //                 list.Add(thisNode);
        //             }
        //         }
        //     }
            
        //     return list;
        // }
    }


    public class TestGraphNewAction : HNGraphNewAction<TestGraph>
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            CreateGraphData();
            Serialize(pathName);
        }


        [MenuItem("Assets/Create/Test Graph")]
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
