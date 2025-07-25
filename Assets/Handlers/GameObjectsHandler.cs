using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Entity.DataContainers;
using UnityEngine;
using Graphics = Assets.Entity.DataContainers.Graphics;

namespace Assets.Handlers
{
    public static class GameObjectsHandler{
        public static Dictionary<string, IObject> Objects = new();
        private static readonly Dictionary<string, Func<string, IObject>> Loaders = new()
        {
            ["Hull"] = path => DataFileHandler.LoadFromJson<HullContainer>(path),
            ["Equipments"] = path => DataFileHandler.LoadFromJson<EquipmentContainer>(path),
            ["Projectiles"] = path => DataFileHandler.LoadFromJson<ProjectileContainer>(path)
        };
        public static void SetupObjectPool(string[] gameObjectsFolderPaths, string[] excludedFolders)
        {
            string[] allFiles = GetFilePaths(gameObjectsFolderPaths, excludedFolders);
            BuildObjects(allFiles);
        }
        private static string[] GetFilePaths(string[] gameObjectsFolderPaths, string[] excludedFolders)
        {
            List<string> allFiles = new();
            foreach (string gameObjectsFolderPath in gameObjectsFolderPaths)
            {
                if (!IsValidPath(gameObjectsFolderPath)) continue;
                IEnumerable<string> files = Directory.GetFiles(gameObjectsFolderPath, "*", SearchOption.AllDirectories)
                    .Where(file => !file.Contains(".meta") && !IsInExcludedFolder(file, excludedFolders));
                allFiles = allFiles.Concat(files).ToList();
            }
            return allFiles.ToArray();
        }
        private static void BuildObjects(string[] allFiles)
        {
            foreach (string filePath in allFiles.Where(file => file.EndsWith(".json")))
            {
                IObject jsonObject = null;
                foreach (var (key, loader) in Loaders)
                {
                    if (filePath.Contains(key))
                    {
                        jsonObject = loader(filePath);
                        break;
                    }
                }
                if (jsonObject == null) continue;
                SetObjectPath(filePath, jsonObject);
                string id = DataFileHandler.GetIdByPath(filePath);
                jsonObject.Id = id;
                Objects.Add(id, jsonObject);
                //Debug.Log(id);
            }
        }
        private static void SetObjectPath(string filePath, IObject jsonObject)
        {
            Graphics graphics = jsonObject.GetGraphics();
            if (graphics == null) return;
            int index = filePath.LastIndexOf('\\');
            filePath = filePath.Substring(0, index) + '\\';
            graphics.Icon = filePath + graphics.Icon;
            for (int i = 0; i < graphics.Textures.Length; i++)
            {
                if (graphics.Textures[i] == "") continue;
                graphics.Textures[i] = filePath + graphics.Textures[i];
                //Debug.Log(graphics.Textures[i]);
            }
        }
        private static bool IsValidPath(string gameObjectsFolderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(gameObjectsFolderPath))
                    throw new Exception("GameObjectsFolderPath is null or empty.");
                if (!Directory.Exists(gameObjectsFolderPath))
                    throw new Exception($"Path does not exist: {gameObjectsFolderPath}");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
            return true;
        }
        private static bool IsInExcludedFolder(string filePath, string[] excludedFolders)
        {
            foreach (var folder in excludedFolders)
            {
                if (filePath.Contains(folder)) return true;
            }
            return false;
        }
    }
}
