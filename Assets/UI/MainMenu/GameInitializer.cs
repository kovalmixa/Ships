using System.IO;
using System.Linq;
using Assets.Handlers;
using Assets.Handlers.Sprite;
using UnityEngine;

namespace Assets.UI.MainMenu
{
    public class GameInitializer : MonoBehaviour
    {
        private string[] _streamingAssetFolderPaths;
        private readonly string[] _excludedFolders = { "IgnoreThisFolder", "Temp" };

        private void Awake()
        {
            _streamingAssetFolderPaths = new[] { Application.streamingAssetsPath, };
            _streamingAssetFolderPaths = _streamingAssetFolderPaths.Concat(GetModFolders()).ToArray();
        }

        private void Start()
        {
            string[] textureFolders = GetPathsByWord(_streamingAssetFolderPaths, "texture");
            TryFormatJsonFilesInFolders(textureFolders);
            string[] gameObjectFolders = GetPathsByWord(_streamingAssetFolderPaths, "object");
            if (textureFolders.Length > 0) GraphicElementHandler.SetupObjectPool(textureFolders, _excludedFolders);
            if (gameObjectFolders.Length > 0) GameObjectsHandler.SetupObjectPool(gameObjectFolders, _excludedFolders);
            //Debug.Log($"Loaded {GameObjectsHandler.Objects.Count} objects.");
        }

        private string[] GetPathsByWord(string[] folderPaths, string part)
        {
            if (folderPaths == null || folderPaths.Length == 0) return null;
            if (part == "") return folderPaths;
            return folderPaths.Where(folderPath => folderPath != "").SelectMany(folderPath => Directory
                    .GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
                    .Where(folder =>
                        !folder.Contains(".meta") && !_excludedFolders.Contains(folder) &&
                        folder.ToLower().Contains(part)))
                .ToArray();
        }

        private string[] GetModFolders()
        {
            string[] paths = { "" };
            return paths;
            //Добавить алгоритм поиска папок с модами в папке модов
        }

        private void TryFormatJsonFilesInFolders(string[] paths)
        {
            foreach (var folderPath in paths)
            {
                string[] files = Directory.GetFiles(folderPath, "*.json");
                foreach (string filePath in files)
                {
                    string content = File.ReadAllText(filePath);
                    int start = content.IndexOf('[');
                    int end = content.LastIndexOf(']');
                    if (start != -1 && end != -1 && start < end)
                    {
                        content = content.Substring(start, end - start + 1);
                        File.WriteAllText(filePath, content);
                    }
                }
            }
        }
    }
}
