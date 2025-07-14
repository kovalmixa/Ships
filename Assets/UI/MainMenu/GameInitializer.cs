using System.Collections;
using System.Collections.Generic;
using Assets.Handlers;
using System.IO;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        string[] searchRoots = {
            Path.Combine(Application.streamingAssetsPath, "GameObjects"),
            //Path.Combine(Application.persistentDataPath,  "Mods")
        };

        string[] excludedFolders = { "IgnoreThisFolder", "Temp" };

        GameObjectsHandler.SetupObjectPool(searchRoots, excludedFolders);

        //Debug.Log($"Loaded {ObjectPoolHandler.Objects.Count} objects.");
    }
}
