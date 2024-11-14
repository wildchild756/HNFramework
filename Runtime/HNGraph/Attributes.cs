using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class HNGraphPortInfoAttribute : Attribute
    {
        public string PortName => name;
        private string name;

        public Orientation orientation;

        public Direction PortDirection => direction;
        private Direction direction;

        public Capacity PortCapacity => capacity;
        private Capacity capacity;


        public HNGraphPortInfoAttribute(string slotName, Orientation orientation, Direction direction, Capacity capacity)
        {
            this.name = slotName;
            this.orientation = orientation;
            this.direction = direction;
            this.capacity = capacity;
        }


        public enum Orientation
        {
            Horizontal,
            Vertical
        }

        public enum Direction
        {
            Input,
            Output
        }

        public enum Capacity
        {
            Single,
            Multi
        }
    }


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
