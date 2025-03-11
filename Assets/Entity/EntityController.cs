using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityController : MonoBehaviour
{
    public string assetObjectPath;
    public bool isPlayer = false;
    public string type = "ship";
    public string hullId;
    [SerializeField] public List<string> weaponIds;
    Entity entity;

    private void Start()
    {
        entity = GetComponent<Entity>();
        if (hullId == "")
        {
            GetDefaultHull();
        }

        LoadHull();
    }
    private void GetDefaultHull()
    {

        switch (type)
        {
            case "ship":
                {
                    hullId = "h_sh_boat";
                    break;
                }
        }
    }

    private void LoadHull()
    {
        string path = DataHandler.GetPathById(hullId, assetObjectPath);
        int index = hullId.LastIndexOf('_');
        string name = hullId.Substring(index + 1); ;
        string JSONFilePath = path + '/' + name + ".json";
        if (!File.Exists(JSONFilePath))
        {
            Debug.LogError($"File not found: {path}.json");
            return;
        }
        HullContainer hullData = DataHandler.LoadFromJson<HullContainer>(JSONFilePath);
        entity.entity.hull = hullData;
    }
    private void Movement()
    {
        if (isPlayer)
        {
            HandleMovement();
        }
    }
    void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) && entity.SpeedLevel < entity.MaxSpeedLevel)
        {
            entity.SpeedLevel++;
        }
        else if (Input.GetKeyDown(KeyCode.S) && entity.SpeedLevel > entity.MinSpeedLevel)
        {
            entity.SpeedLevel--;
        }

        float rotationDirection = Input.GetAxis("Horizontal");
        entity.Movement(rotationDirection);
    }
    private void Update()
    {
        Movement();
    }
}
