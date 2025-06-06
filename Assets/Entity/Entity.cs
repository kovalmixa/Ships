using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Entity.DataContainers;
using Assets.InGameMarkers.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Assets.Entity
{
    public class Entity : MonoBehaviour
    {
        public EntityController EntityController;
        public EntityContainer EntityData = new();
        public Transform HullLayers;
        private List<Sprite> _hullSprites = new();
        public Vector2 Size
        {
            get => GetComponent<BoxCollider2D>().size;
            set => GetComponent<BoxCollider2D>().size = value;
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
            script?.Execute(this);
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
            float totalMass = rb.mass + otherRb.mass; // Общая масса двух объектов
            float impulse = _currentSpeed * rb.mass;  // Импульс игрока

            // Передаем часть импульса другому объекту
            otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);

            //Теряет скорость пропорционально массе другого объекта
            _currentSpeed *= otherRb.mass / totalMass;
        }
        public void SetupHullLayers(HullContainer hull)
        {
            EntityData.HullData = hull;
            string[] texturePaths = hull.Graphics.Textures;
            StartCoroutine(SetupLayersCoroutine(texturePaths));
        }
        private void SetColliderSize()
        {
            Vector2 maxSize = new();
            foreach (Sprite sprite in _hullSprites)
            {
                Vector2 textureSize = new(sprite.texture.width, sprite.texture.height);
                float ppu = sprite.pixelsPerUnit;
                Vector2 worldSize = textureSize / ppu;

                maxSize.x = Mathf.Max(maxSize.x, worldSize.x);
                maxSize.y = Mathf.Max(maxSize.y, worldSize.y);
            }

            Size = maxSize * 1.2f;
            Debug.Log($"Collider size set to: {Size}");
        }

        private IEnumerator SetupLayersCoroutine(string[] texturePaths)
        {
            for (int i = 0; i < texturePaths.Length; i++)
            {
                GameObject layerGo = new GameObject($"textureLayer{i}");
                SpriteRenderer rend = layerGo.AddComponent<SpriteRenderer>();
                bool done = false;
                Sprite loaded = null;
                yield return StreamingSpriteLoader.LoadSprite(texturePaths[i], s => {
                    loaded = s;
                    done = true;
                });
                if (!done || loaded == null)
                {
                    Destroy(layerGo);
                    continue;
                }
                rend.sprite = loaded;
                _hullSprites.Add(rend.sprite);
                layerGo.transform.SetParent(HullLayers, false);
                layerGo.transform.localPosition = Vector3.zero;
            }
            var baseRenderer = GetComponent<SpriteRenderer>();
            if (baseRenderer) baseRenderer.forceRenderingOff = true;
            SetColliderSize();
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
