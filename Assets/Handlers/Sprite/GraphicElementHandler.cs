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

        public static void SetupObjectPool(string[] gameObjectsFolderPaths, string[] excludedFolders)
        {
            string[] allFiles = FilesExtractor.GetFilesPaths(gameObjectsFolderPaths, excludedFolders);
            BuildObjects(allFiles);
        }

        private static void BuildObjects(string[] allFiles)
        {
            foreach (string filePath in allFiles.Where(file => file.EndsWith(".json")))
            {
                GraphicElement[] jsonObject = DataFileHandler.LoadFromJson<GraphicElement[]>(filePath);
                if (jsonObject == null) continue;
                string tag = filePath.Split('\\')[^1];
                tag = tag.Substring(0, tag.IndexOf('.'));
                foreach (GraphicElement graphicElement in jsonObject)
                {
                    string key = $"{tag}_{graphicElement.Filename}";
                    //Debug.Log(key);
                    Objects.Add(key, graphicElement);
                }
            }
            //Debug.Log(Objects["USA_Duck.png"].Frame.Width);

        }
    }
}
