using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : MonoBehaviour
{
    public bool isPlayer = false;

    public EntityContainer entity;
    public HullContainer hull;
    public Transform HullLayers;

    public float maxSpeed = 5f;
    public float acceleration = 3f; // ��������� ������ ���������� ��������
    public float rotationSpeed = 60f;

    private float targetSpeed = 0f; // �������� �������� (���������� ��� ��������)
    private float currentSpeed = 0f; // ������� ��������
    private int speedLevel = 0;
    private int maxSpeedLevel = 3; // 3 ������ �������� ������
    private int minSpeedLevel = -1; // 1 ������� �����

    public float bounceForce = 5f; // ���� ������������

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        boxCollider.size = spriteSize / transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Rigidbody2D otherRb = collision.rigidbody;

        if (otherRb != null) // ���������, ���� �� � ������� ������� Rigidbody2D
        {
            // ��������� ����������� ������������
            Vector2 pushDirection = (rb.position - otherRb.position).normalized;

            // ��������� ���� ������������ ����� ��������
            rb.AddForce(pushDirection * bounceForce, ForceMode2D.Impulse);
            otherRb.AddForce(-pushDirection * bounceForce, ForceMode2D.Impulse);
            print(name);
        }
    }
    private void Move() {
        if (isPlayer)
        {
            HandleMovement();
        }
    }
    void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) && speedLevel < maxSpeedLevel)
        {
            speedLevel++;
        }
        else if (Input.GetKeyDown(KeyCode.S) && speedLevel > minSpeedLevel)
        {
            speedLevel--;
        }

        targetSpeed = speedLevel * (maxSpeed / maxSpeedLevel); // ��������� ������� ��������

        // ������� ��������� ��������
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // ���������� ���������
        float rotationDirection = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward, -rotationDirection * rotationSpeed * Time.deltaTime);

        // �����������
        transform.Translate(Vector3.up * currentSpeed * Time.deltaTime, Space.Self);
    }
    void Update()
    {
        Move();
    }
}
