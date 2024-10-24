using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HN.Graph
{
    public abstract class HNGraphObject : ScriptableObject
    {
        public string Extension => extension;
        private string extension;


        public HNGraphObject(string extension)
        {
            this.extension = extension;
        }



#if UNITY_EDITOR

#endif


    }
}
