using System;
using Assets.Entity.DataContainers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Handlers.FileHandlers;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;

namespace Assets.Handlers.Sprite
{
    public static class GraphicElementHandler
    {
        public static Dictionary<string, GraphicElement> Objects = new();

        public static void SetupObjectPool(string[] gameObjectsFolderPaths, string[] excludedPaths)
        {
            string[] allFiles = FilesExtractor.GetFilesPaths(gameObjectsFolderPaths, excludedPaths);
            allFiles = allFiles.Where(file => excludedPaths.Any(path => !file.Contains(path))).ToArray();
            BuildObjects(allFiles);
        }

        private static void BuildObjects(string[] allFiles)
        {
            foreach (string filePath in allFiles.Where(file => file.EndsWith(".json")))
            {
                string atlasPath = filePath[..filePath.IndexOf(".json", StringComparison.Ordinal)]+".png";
                GraphicElement[] graphicElements = DataFileHandler.LoadFromJson<GraphicElement[]>(filePath);
                if (graphicElements == null || graphicElements.Length == 0) continue;
                string tag = filePath.Split('\\')[^1];
                tag = tag.Substring(0, tag.IndexOf('.'));
                foreach (GraphicElement graphicElement in graphicElements)
                {
                    string name = graphicElement.Filename;
                    string key = $"{tag}_{name[..name.IndexOf(".png", StringComparison.Ordinal)]}";
                    graphicElement.Filename = key;
                    graphicElement.SpriteAtlasPath = atlasPath;
                    Objects.Add(key, graphicElement);
                    Debug.Log(key);
                }
            }
            //Debug.Log(Objects["USA_Duck.png"].Frame.Width);
        }

    }
}
