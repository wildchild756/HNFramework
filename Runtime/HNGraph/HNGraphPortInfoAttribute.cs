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

        public Direction PortDirection => direction;
        private Direction direction;

        public Capacity PortCapacity => capacity;
        private Capacity capacity;


        public HNGraphPortInfoAttribute(string slotName, Direction direction, Capacity capacity)
        {
            this.name = slotName;
            this.direction = direction;
            this.capacity = capacity;
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
}
