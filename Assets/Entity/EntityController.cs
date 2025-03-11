using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityController : MonoBehaviour
{
    public bool isPlayer = false;
    Entity entity;

    private void Start()
    {
        entity = GetComponent<Entity>();
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
