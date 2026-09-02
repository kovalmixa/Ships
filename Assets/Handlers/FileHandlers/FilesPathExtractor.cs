using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Handlers.FileHandlers
{
    public static class FilesPathExtractor
    {
        public static IEnumerable<string> GetFilePaths(IEnumerable<string> folderPaths, IEnumerable<string>? excludedFolders = null)
        {
            excludedFolders ??= Enumerable.Empty<string>();

            return folderPaths
                .Where(IsValidPath)
                .SelectMany(folderPath => Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                .Where(file => !file.Contains(".meta") && !IsInExcludedFolder(file, excludedFolders));
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
        private static bool IsInExcludedFolder(string filePath, IEnumerable<string> excludedFolders)
        {
            foreach (var folder in excludedFolders) if (filePath.Contains(folder)) return true;
            return false;
        }
    }
}
