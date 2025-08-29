using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Handlers;
using Assets.Handlers.Sprite;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.UI.MainMenu
{
    public class GameInitializer : MonoBehaviour
    {
        private string[] _streamingAssetFolderPaths;
        private readonly List<string> _excludedPaths = new()
        {
            "IgnoreThisFolder",
            "Temp"
        };
        private string GraphicSettingsPattern = "graphicSettings";

        private void Awake()
        {
            _excludedPaths.Add(GraphicSettingsPattern);
            _streamingAssetFolderPaths = new[] { Application.streamingAssetsPath, };
            _streamingAssetFolderPaths = _streamingAssetFolderPaths.Concat(GetModFolders()).ToArray();
        }

        private void Start()
        {
            string[] textureFolders = GetPathsByWord(_streamingAssetFolderPaths, "texture");
            TryFormatTextureJsonFiles(textureFolders);
            string[] gameObjectFolders = GetPathsByWord(_streamingAssetFolderPaths, "object");
            if (textureFolders.Length > 0) 
                GraphicElementHandler.SetupObjectPool(textureFolders, _excludedPaths.ToArray());
            if (gameObjectFolders.Length > 0) 
                GameObjectsHandler.SetupObjectPool(gameObjectFolders, _excludedPaths.ToArray());
            ConfigureGraphicElements(textureFolders);
            //Debug.Log($"Loaded {GameObjectsHandler.Objects.Count} objects.");
        }

        private void ConfigureGraphicElements(string[] textureFolders)
        {
            foreach (var folder in textureFolders)
            {
                string[] files = Directory.GetFiles(folder, $"*{GraphicSettingsPattern}*.json");
                string jsonString = File.ReadAllText(files[0]);
                GraphicElement[] graphicElements = JsonConvert.DeserializeObject<GraphicElement[]>(jsonString);
                foreach (var graphicElementT in graphicElements)
                {
                    GraphicElement graphicElement = GraphicElementHandler.Objects[graphicElementT.Filename];
                    if (graphicElement == null)
                    {
                        Debug.LogError($"Graphic element not found: {graphicElementT}");
                        continue;
                    }
                    graphicElement.FPS = graphicElementT.FPS;
                    graphicElement.Quantity = graphicElementT.Quantity;
                    //Debug.Log($"Name: {graphicElement.Filename} Time: {graphicElement.Time}");
                }
            }
        }

        private string[] GetPathsByWord(string[] folderPaths, string part)
        {
            if (folderPaths == null || folderPaths.Length == 0) return null;
            if (part == "") return folderPaths;
            return folderPaths.Where(folderPath => folderPath != "").SelectMany(folderPath => Directory
                    .GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly)
                    .Where(folder =>
                        !folder.Contains(".meta") && !_excludedPaths.Contains(folder) &&
                        folder.ToLower().Contains(part)))
                .ToArray();
        }

        private string[] GetModFolders()
        {
            string[] paths = { "" };
            return paths;
            //Добавить алгоритм поиска папок с модами в папке модов
        }

        private void TryFormatTextureJsonFiles(string[] paths)
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
