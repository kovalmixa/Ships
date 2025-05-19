using System;
using Assets.Entity.DataContainers;
using UnityEngine;

namespace Assets.Entity
{
    public class Entity : MonoBehaviour
    {
        public EntityContainer EntityData = new EntityContainer();
        public Transform HullLayers;

        public float MaxSpeed = 5f;
        public float Acceleration = 3f;
        public float RotationSpeed = 60f;

        private float _targetSpeed = 0f;
        private float _currentSpeed = 0f;
        private int _speedLevel = 0;
        public int SpeedLevel{ 
            get => _speedLevel;
            set
            {
                _speedLevel = value;
            }
        }
        private int _maxSpeedLevel = 3;
        public int MaxSpeedLevel
        {
            get => _maxSpeedLevel;
        }
        private int _minSpeedLevel = -1;
        public int MinSpeedLevel
        {
            get => _minSpeedLevel;
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
                _speedLevel = 0;
                return;
            }
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 pushDirection = (rb.position - otherRb.position).normalized;
            float totalMass = rb.mass + otherRb.mass; // Общая масса двух объектов
            float impulse = _currentSpeed * rb.mass;  // Импульс игрока

            // Передаем часть импульса другому объекту
            otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);

            // Игрок теряет скорость пропорционально массе другого объекта
            _currentSpeed *= otherRb.mass / totalMass;
        }
        public void Movement(float rotationDirection)
        {
            _targetSpeed = _speedLevel * (MaxSpeed / _maxSpeedLevel);
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, Acceleration * Time.deltaTime);

            transform.Rotate(Vector3.forward, -rotationDirection * RotationSpeed * Time.deltaTime);

            transform.Translate(Vector3.up * _currentSpeed * Time.deltaTime, Space.Self);
        }
    }
}
