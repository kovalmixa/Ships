using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Handlers
{
    public static class ObjectPoolHandler{

        public static Dictionary<string, object> Objects;

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
            foreach (string file in allFiles.Where(file => file.EndsWith(".json")))
            {
                //Debug.Log(file);
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
