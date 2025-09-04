using System;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;

namespace Assets.Handlers.FileHandlers
{
    public static class DataFileHandler
    {
        public static void SaveToJson<T>(T data, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                System.IO.File.WriteAllText(filePath, json);
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
                if (!System.IO.File.Exists(filePath))
                {
                    Debug.LogError($"File not found: {filePath}");
                    return default;
                }

                string json = System.IO.File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading JSON on {filePath}: {ex.Message}");
                return default;
            }
        }
    }
}