using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace HN.Serialize
{
    [Serializable]
    public abstract class JsonObject
    {
        public bool Dirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }


        private bool isDirty = false;


        public string SerializeToJson()
        {            
            return Json.Serialize(this);
        }

        public void DeserializeFromString(string jsonString)
        {
            Json.DeserializeFromString(this, jsonString);
        }
    }
}
