using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Entity.DataContainers;
using Assets.Handlers.FileHandlers;
using Graphics = Assets.Entity.DataContainers.Graphics;

namespace Assets.Handlers
{
    public static class GameObjectsHandler
    {
        public static Dictionary<string, IObject> Objects = new();
        private static readonly Dictionary<string, Func<string, IObject>> Loaders = new()
        {
            ["Hull"] = path => DataFileHandler.LoadFromJson<HullContainer>(path),
            ["Equipments"] = path => DataFileHandler.LoadFromJson<EquipmentContainer>(path),
            ["Projectiles"] = path => DataFileHandler.LoadFromJson<ProjectileContainer>(path),
            ["Effects"] = path => DataFileHandler.LoadFromJson<EffectContainer>(path)
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
    }
}
