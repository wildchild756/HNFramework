using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace HN.Graph.Editor
{
    public abstract class HNGraphSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public HNGraphView graph;
        public HNGraphNode target;

        public static List<SearchContextElement> elements;


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Nodes")));
            List<SearchContextElement> elements = FindElements();
            SortElements(elements);
            CreateSearchTreeEntry(tree);
            return tree;
        }


        public abstract List<SearchContextElement> FindElements();

        private void SortElements(List<SearchContextElement> elements)
        {
            elements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.title.Split('/');
                string[] splits2 = entry2.title.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length)
                    {
                        return 1;
                    }
                    int value = splits1[i].CompareTo(splits2[i]);
                    if (value != 0)
                    {
                        if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                        {
                            return splits1.Length < splits2.Length ? 1 : -1;
                        }
                        return value;
                    }
                }
                return 0;
            });
        }

        private void CreateSearchTreeEntry(List<SearchTreeEntry> tree)
        {
            List<string> groups = new List<string>();
            foreach (SearchContextElement element in elements)
            {
                string[] entryTitle = element.title.Split('/');
                string groupName = "";
                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
                entry.level = entryTitle.Length;
                entry.userData = new SearchContextElement(element.target, element.title);
                tree.Add(entry);
            }

        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = graph.ChangeCoordinatesTo(graph, context.screenMousePosition - graph.graphEditorWindow.position.position);
            var graphMousePosition = graph.contentViewContainer.WorldToLocal(windowMousePosition);

            SearchContextElement element = (SearchContextElement)searchTreeEntry.userData;

            HNRenderPass pass = (HNRenderPass)element.target;
            HNGraphNode node = new HNGraphNode(pass);
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
            graph.AddNode(node);

            return true;
        }


        public struct SearchContextElement
        {
            public object target { get; private set; }
            public string title { get; private set; }

            public SearchContextElement(object target, string title)
            {
                this.target = target;
                this.title = title;
            }
        }

    }
}
