using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace HN.Serialize
{
    public class Json
    {
        public static bool Serialize(Object obj, string path)
        {
            if(string.IsNullOrEmpty(path) || obj == null)
            {
                return false;
            }
            FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
            file.Seek(0, SeekOrigin.Begin);
            file.SetLength(0);
            file.Close();
            StreamWriter sw;
            string jsonString = JsonUtility.ToJson(obj);
            sw = File.CreateText(path);
            sw.Write(jsonString);
            sw.Close();
            sw.Dispose();
            return true;
        }

        public static string Serialize(Object obj)
        {
            if(obj == null)
            {
                return string.Empty;
            }
            return JsonUtility.ToJson(obj);
        }

        public static bool Deserialize<T>(T obj, string path)
        {
            if(string.IsNullOrEmpty(path) || obj == null)
            {
                return false;
            }
            string jsonString = File.ReadAllText(path, Encoding.UTF8);
            JsonUtility.FromJsonOverwrite(jsonString, obj);
            return true;
        }

        public static void DeserializeFromString<T>(T obj, string jsonString)
        {
            if(string.IsNullOrEmpty(jsonString))
            {
                jsonString = "{}";
            }
            JsonUtility.FromJsonOverwrite(jsonString, obj);
        }
    }
}
