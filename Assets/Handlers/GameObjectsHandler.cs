using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Entity.DataContainers;
using Assets.Handlers.FileHandlers;
using UnityEngine;
using Graphics = Assets.Entity.DataContainers.Graphics;

namespace Assets.Handlers
{
    public static class GameObjectsHandler
    {
        public static Dictionary<string, IObject> Objects = new();
        private static readonly Dictionary<string, Func<string, IObject>> Loaders = new()
        {
            ["Hull"] = DataFileHandler.LoadFromJson<HullContainer>,
            ["Equipments"] = DataFileHandler.LoadFromJson<EquipmentContainer>,
            ["Projectiles"] = DataFileHandler.LoadFromJson<ProjectileContainer>,
            ["Effects"] = DataFileHandler.LoadFromJson<EffectContainer>
        };
        public static void SetupObjectPool(string[] gameObjectsFolderPaths, string[] excludedFolders)
        {
            string[] allFiles = FilesExtractor.GetFilesPaths(gameObjectsFolderPaths, excludedFolders);
            BuildObjects(allFiles);
        }
        private static void BuildObjects(string[] allFiles)
        {
            foreach (string filePath in allFiles.Where(file => file.EndsWith(".json")))
            {
                IObject jsonObject = null;
                foreach (var (key, loader) in Loaders)
                {
                    if (!filePath.Contains(key)) continue;
                    jsonObject = loader(filePath);
                    break;
                }
                if (jsonObject == null) continue;
                string id = DataFileHandler.GetIdByPath(filePath);
                jsonObject.Id = id;
                Objects.Add(id, jsonObject);
                //Debug.Log(id);
            }
        }
    }
}
