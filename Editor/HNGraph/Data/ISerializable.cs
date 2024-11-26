using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HN.Graph.Editor
{
    public interface ISerializable
    {
        public string Serialize();
        
        public void Deserialize(string serializedData);
    }
}
