using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HN.Serialize;
using UnityEngine;
using UnityEngine.UIElements;

namespace HN.Graph
{
    public class HNGraphUtils
    {
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }



        public static VisualElement DrawProperty(HNGraphInspectableInfo attribute, JsonObject jsonObject, PropertyInfo propertyInfo)
        {
            return attribute.Inspect(jsonObject, propertyInfo);
        }

        public static VisualElement DrawProperties(JsonObject jsonObject, BindingFlags bindingFlags)
        {
            VisualElement root = new VisualElement();

            Type objType = jsonObject.GetType();
            foreach(var propertyInfo in objType.GetProperties(bindingFlags))
            {
                foreach(HNGraphInspectableInfo attribute in propertyInfo.GetCustomAttributes(typeof(HNGraphInspectableInfo), false))
                {
                    var propertyField = DrawProperty(attribute, jsonObject, propertyInfo);
                    root.Add(propertyField);
                }
            }

            return root;
        }
    }


}
