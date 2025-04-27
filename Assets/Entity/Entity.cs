using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : MonoBehaviour
{
    public EntityContainer entityData = new EntityContainer();
    public Transform HullLayers;

    public float maxSpeed = 5f;
    public float acceleration = 3f;
    public float rotationSpeed = 60f;

    private float targetSpeed = 0f;
    private float currentSpeed = 0f;
    private int speedLevel = 0;
    public int SpeedLevel{ 
        get => speedLevel;
        set
        {
            speedLevel = value;
        }
    }
    private int maxSpeedLevel = 3;
    public int MaxSpeedLevel
    {
        get => maxSpeedLevel;
    }
    private int minSpeedLevel = -1;
    public int MinSpeedLevel
    {
        get => minSpeedLevel;
    }
    void Start()
    {
        SetColliderSize();
    }
    private void SetColliderSize()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        boxCollider.size = spriteSize / transform.localScale * 1.2f;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.rigidbody;
        if (otherRb == null)
        {
            return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            speedLevel = 0;
            return;
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 pushDirection = (rb.position - otherRb.position).normalized;
        float totalMass = rb.mass + otherRb.mass; // Общая масса двух объектов
        float impulse = currentSpeed * rb.mass;  // Импульс игрока

        // Передаем часть импульса другому объекту
        otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);

        // Игрок теряет скорость пропорционально массе другого объекта
        currentSpeed *= otherRb.mass / totalMass;
    }
    public void Movement(float rotationDirection)
    {
        targetSpeed = speedLevel * (maxSpeed / maxSpeedLevel);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        transform.Rotate(Vector3.forward, -rotationDirection * rotationSpeed * Time.deltaTime);

        transform.Translate(Vector3.up * currentSpeed * Time.deltaTime, Space.Self);
    }
}
