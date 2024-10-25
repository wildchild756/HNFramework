using System.Collections;
using System.Collections.Generic;
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

    }
}
