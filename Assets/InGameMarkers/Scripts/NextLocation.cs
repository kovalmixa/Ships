using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Assets.Entity;
using Assets.Handlers.SceneHandlers;
using Assets.InGameMarkers.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class NextLocationArea : ScriptBase
{
    public string LocationName;
    private GameObject _sceneController;
    private void Awake()
    {
        _sceneController = GameObject.Find("Handlers");
        if (_sceneController == null) Debug.LogWarning("SceneController not found");
    }
    public override bool Execute(EntityBody entityBody)
    {
        if (entityBody.EntityController.IsPlayer)
        {
            _sceneController.GetComponent<SceneController>().NextLocation(LocationName);
            entityBody.EntityController.transform.position = Vector3.zero;
            return true;
        }
        return false;
    }

    public override bool IsFinished(EntityBody entityBody) => false;

    public override bool IsExecuted(EntityBody entityBody) => false;
}
