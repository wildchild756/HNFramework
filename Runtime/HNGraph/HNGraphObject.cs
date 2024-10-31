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

        public string SerializedEditorData
        {
            get { return serializedEditorData; }
            set { serializedEditorData = value; }
        }
        [SerializeField]
        private string serializedEditorData = "{}";


#if UNITY_EDITOR
        public abstract void Serialize();
#endif

        public HNGraphObject()
        {
            
        }

    }
}
