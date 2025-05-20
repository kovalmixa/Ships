using System;
using System.IO;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

namespace Assets.Handlers
{
    public static class DataHandler
    {
        public static void SaveToJson<T>(T data, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Debug.Log($"Data saved to {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving JSON: {ex.Message}");
            }
        }

        public static T LoadFromJson<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"File not found: {filePath}");
                    return default;
                }

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading JSON: {ex.Message}");
                return default;
            }
        }

        public static string GetPathById(string id, string assetPath)
        {
            string path = assetPath;
            string[] objectTypePath = id.Split('_');
            GetObjectPath(objectTypePath, ref path);
            path += '/';
            return path += objectTypePath[objectTypePath.Length - 1];
        }

        private static void GetObjectPath(string[] objectTypePath, ref string path)
        {
            path += '/' + objectTypePath[0];
            SwitchObjectTypePath(objectTypePath, ref path);
        }
        private static void SwitchObjectTypePath(string[] objectTypePath, ref string path)
        {
            switch (objectTypePath[1])
            {
                case "h":
                {
                    path += "/Hulls";
                    SwitchHullTypePath(objectTypePath, ref path);
                    break;
                }
                case "w":
                {
                    path += "/Weapons";
                    break;
                }
                case "p":
                {
                    path += "/Projectiles";
                    break;
                }
            }
        }

        private static void SwitchHullTypePath(string[] objectTypePath, ref string path)
        {
            switch (objectTypePath[2])
            {
                case "sh":
                {
                    path += "/Ships";
                    break;
                }
            }
        }
    }
}