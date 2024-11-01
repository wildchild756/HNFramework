using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HN
{
    [Serializable]
    public class SerializedDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<K> keys = new List<K>();

        [SerializeField]
        List<V> values = new List<V>();


        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach(var kv in this)
            {
                keys.Add(kv.Key);
                values.Add(kv.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            for(int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }

            keys.Clear();
            values.Clear();
        }
    }
}
