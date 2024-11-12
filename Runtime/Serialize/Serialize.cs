using System;
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
        public static bool Serialize(System.Object obj, string path)
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

        public static string Serialize(System.Object obj)
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

        public static System.Object DeserializeFromString(string typeName, string jsonString)
        {
            if(string.IsNullOrEmpty(jsonString))
            {
                return null;
            }
            var type = Type.GetType(typeName);
            var obj = Activator.CreateInstance(type);
            if(obj == null)
            {
                return null;
            }
            DeserializeFromString(obj, jsonString);
            return obj;
        }

        public static T Deserialize<T>(string path)
        {
            var obj = Activator.CreateInstance<T>();
            if(Deserialize(obj, path))
            {
                return obj;
            }
            return default;
        }

        public static T DeserializeFromString<T>(string jsonString)
        {
            var obj = Activator.CreateInstance<T>();
            DeserializeFromString(obj, jsonString);

            return obj;
        }
    }
}
