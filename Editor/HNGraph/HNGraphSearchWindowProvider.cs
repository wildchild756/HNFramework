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
    public class HNGraphSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        public HNGraphView graph;
        public HNGraphNode target;

        public static List<SearchContextElement> elements;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Nodes")));

            elements = new List<SearchContextElement>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.CustomAttributes.ToList() != null)
                    {
                        var attribute = type.GetCustomAttribute(typeof(HNGraphNodeInfoAttribute));
                        if (attribute != null)
                        {
                            HNGraphNodeInfoAttribute attr = (HNGraphNodeInfoAttribute)attribute;
                            var node = Activator.CreateInstance(type);

                            if (string.IsNullOrEmpty(attr.MenuItem))
                            {
                                continue;
                            }
                            elements.Add(new SearchContextElement(node, attr.MenuItem));
                        }
                    }
                }
            }

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


            return tree;
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

        private void AddToSearchTree(string title, Type type, List<SearchTreeEntry> entries, Assembly[] assemblies)
        {
            entries.Add(new SearchTreeGroupEntry(new GUIContent(title), 1));
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.IsClass && !t.IsAbstract && t.IsSubclassOf(type))
                    {
                        AddPass(entries, t);
                    }
                }
            }
        }

        private void AddPass(List<SearchTreeEntry> entries, Type type)
        {
            if (Activator.CreateInstance(type) is HNRenderPass entry)
            {
                entries.Add(new SearchTreeEntry(new GUIContent(entry.passName))
                {
                    level = 2,
                    userData = entry,
                });
            }
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
