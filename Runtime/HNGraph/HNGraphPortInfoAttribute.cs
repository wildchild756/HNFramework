using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class HNGraphPortInfoAttribute : Attribute
    {
        public string SlotName => slotName;
        private string slotName;

        public SlotType Type => type;
        private SlotType type;


        public HNGraphPortInfoAttribute(string slotName, SlotType type)
        {
            this.slotName = slotName;
            this.type = type;
        }


        public enum SlotType
        {
            Input,
            Output
        }
    }
}
