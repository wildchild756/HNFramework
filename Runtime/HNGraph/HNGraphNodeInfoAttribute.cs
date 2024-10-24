using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class HNGraphNodeInfoAttribute : Attribute
    {
        public string NodeTitle => nodeTitle;
        private string nodeTitle;

        public string MenuItem => menuItem;
        private string menuItem;

        public HNGraphNodeInfoAttribute(string nodeTitle, string menuItem = "")
        {
            this.nodeTitle = nodeTitle;
            this.menuItem = menuItem;
        }
    }
}
