using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Handlers.FileHandlers
{
    public static class FilesExtractor
    {
        public static string[] GetFilesPaths(string[] folderPaths, string[] excludedFolders)
        {
            List<string> allFiles = new();
            allFiles = (from folderPath in folderPaths
                where IsValidPath(folderPath)
                select Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories)
                    .Where(file => !file.Contains(".meta") && !IsInExcludedFolder(file, excludedFolders))).Aggregate(allFiles, (current, files) => current.Concat(files).ToList());
            return allFiles.ToArray();
        }
        private static bool IsValidPath(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                    throw new Exception("GameObjectsFolderPath is null or empty.");
                if (!Directory.Exists(folderPath))
                    throw new Exception($"Path does not exist: {folderPath}");
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
