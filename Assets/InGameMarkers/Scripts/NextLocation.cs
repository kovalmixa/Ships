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
    public override bool Execute(EntityController entityController)
    {
        if (entityController.IsPlayer)
        {
            _sceneController.GetComponent<SceneController>().NextLocation(LocationName);
            entityController.transform.position = Vector3.zero;
            return true;
        }
        return false;
    }

    public override bool IsFinished(EntityController entityController) => false;

    public override bool IsExecuted(EntityController entityController) => false;
}
