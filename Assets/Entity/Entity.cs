using System;
using Assets.Entity.DataContainers;
using Assets.GameObjects.InGameMarkers.Scripts;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Assets.Entity
{
    public class Entity : MonoBehaviour
    {
        public EntityController EntityController;
        public EntityContainer EntityData = new EntityContainer();
        public Transform HullLayers;
        public Vector2 Size
        {
            get => GetComponent<BoxCollider2D>().size;
            set
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
                Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
                boxCollider.size = spriteSize / transform.localScale * value;
            }
        }

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
            Size = new Vector2(1.2f,1.2f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IScript script = other.GetComponent<IScript>();
            script.Execute(this);
        }
        private void OnCollisionEnter2D(Collision2D collision)
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
            float totalMass = rb.mass + otherRb.mass; // ����� ����� ���� ��������
            float impulse = _currentSpeed * rb.mass;  // ������� ������

            // �������� ����� �������� ������� �������
            otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);

            //������ �������� ��������������� ����� ������� �������
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
