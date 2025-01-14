using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph
{
    [Serializable]
    public abstract class HNGraphObject : ScriptableObject
    {
        public string AssetPath
        {
            get { return assetPath; }
            set { assetPath = value; }
        }
        
        
        [SerializeField]
        private string assetPath;


    }
}
