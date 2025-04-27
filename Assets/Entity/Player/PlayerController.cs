using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerController : MonoBehaviour, IEntityController
{
    public void UpdateControl(Entity entity)
    {
        MoveControl(entity);
        RotateControl(entity);
        AttackControl(entity);
    }
    private void MoveControl(Entity entity)
    {
        if (Input.GetKeyDown(KeyCode.W) && entity.SpeedLevel < entity.MaxSpeedLevel)
            entity.SpeedLevel++;
        else if (Input.GetKeyDown(KeyCode.S) && entity.SpeedLevel > entity.MinSpeedLevel)
            entity.SpeedLevel--;

        float rotationInput = Input.GetAxis("Horizontal");
        entity.Movement(rotationInput);
    }
    private void AttackControl(Entity entity)
    {
    }
    private void RotateControl(Entity entity)
    {
    }
}