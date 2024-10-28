using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph
{
    public abstract class HNGraphObject : ScriptableObject
    {
        public string AssetPath
        {
            get { return assetPath; }
            set { assetPath = value; }
        }
        [SerializeField]
        private string assetPath;

        public string EditorDataJson
        {
            get { return editorDataJson; }
            set { editorDataJson = value; }
        }
        [SerializeField]
        private string editorDataJson = "{}";


#if UNITY_EDITOR
        public abstract void SerializeToJson();
#endif

        public HNGraphObject()
        {
            
        }

    }
}
