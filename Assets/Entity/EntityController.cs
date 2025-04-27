using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public string assetObjectPath;
    public bool isPlayer = false;
    public string AIName { get; set; }
    public string type = "ship";
    public string hullId;
    [SerializeField] public List<GameObject> RouteScriptsList;
    [SerializeField] public List<GameObject> ScriptAreaList;
    [SerializeField] public List<GameObject> ScriptList; //реализовать как скрипты
    [SerializeField] public List<string> weaponIds;
    private Entity entity;
    private IEntityController controller;
    private void Start()
    {
        entity = GetComponent<Entity>();
        if (hullId == "")
            GetDefaultHull();

        LoadHull();

        if (isPlayer)
            controller = gameObject.AddComponent<PlayerController>();
        else
        {
            controller = gameObject.AddComponent<AIController>();
            AIController aiController = controller as AIController;
            aiController.SetupRouteScripts(RouteScriptsList);
        }
    }

    private void Update()
    {
        if (controller != null)
            controller.UpdateControl(entity);
    }
    private void GetDefaultHull()
    {
        switch (type)
        {
            case "ship":
                hullId = "h_sh_boat";
                break;
        }
    }
    private void LoadHull()
    {
        string path = DataHandler.GetPathById(hullId, assetObjectPath);
        int index = hullId.LastIndexOf('_');
        string name = hullId.Substring(index + 1);
        string JSONFilePath = path + '/' + name + ".json";

        if (!File.Exists(JSONFilePath))
        {
            Debug.LogError($"File not found: {JSONFilePath}");
            return;
        }

        HullContainer hullData = DataHandler.LoadFromJson<HullContainer>(JSONFilePath);
        entity.entityData.hullData = hullData;
    }
}