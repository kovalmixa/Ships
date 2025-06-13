using System.Collections;
using System.Collections.Generic;
using Assets.Entity.DataContainers;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using Assets.InGameMarkers.Scripts;

namespace Assets.Entity
{
    public class InGameObject : MonoBehaviour
    {
        public string Type
        {
            get => LayerMask.LayerToName(gameObject.layer);
            set
            {
                int layer = LayerMask.NameToLayer(value);
                if (layer != -1)
                {
                    gameObject.layer = layer;
                }
                else
                {
                    Debug.LogWarning($"Unknown layer name: {value}");
                }
            }
        }
        public Transform LayersAnchor;
        protected float Speed { get; set; }
        protected float CurrentSpeed { get; set; }

        protected bool IsTrigger = false;
        protected bool IsComplexCollision = false;
        public Vector2 Size
        {
            get
            {
                var collider = GetComponent<BoxCollider2D>();
                if (collider == null)
                    collider = gameObject.AddComponent<BoxCollider2D>();
                return collider.size;
            }
            set
            {
                var collider = GetComponent<BoxCollider2D>();
                if (collider == null)
                    collider = gameObject.AddComponent<BoxCollider2D>();
                collider.size = value;
            }
        }

        protected List<GameObject> Layers = new();
        protected void SetColliderSize()
        {
            Vector2 maxSize = new(0,0);
            foreach (GameObject layer in Layers)
            {
                Sprite sprite = layer.GetComponent<SpriteRenderer>().sprite;
                if (sprite == null) continue;
                Vector2 textureSize = GetTextureSizeFromSprite(sprite);
                maxSize.x = Mathf.Max(maxSize.x, textureSize.x);
                maxSize.y = Mathf.Max(maxSize.y, textureSize.y);
            }
            Size = maxSize / 10;
            if (IsComplexCollision) SetAllColliders();
        }
        private void SetAllColliders()
        {
            foreach (GameObject layer in Layers)
            {
                Sprite sprite = layer.GetComponent<SpriteRenderer>().sprite;
                if (sprite == null)
                {
                    layer.AddComponent<BoxCollider2D>().size = new(0, 0);
                    continue;
                }
                Vector2 textureSize = GetTextureSizeFromSprite(sprite);
                layer.AddComponent<BoxCollider2D>().size = textureSize;
            }
        }
        private Vector2 GetTextureSizeFromSprite(Sprite sprite)
        {
            Vector2 textureSize = new(sprite.texture.width, sprite.texture.height);
            float ppu = sprite.pixelsPerUnit;
            textureSize = textureSize / ppu;
            return textureSize;
        }
        protected IEnumerator SetupLayersCoroutine(string[] texturePaths)
        {
            for (int i = 0; i < texturePaths.Length; i++)
            {
                GameObject layerGo = new GameObject($"textureLayer{i}");
                SpriteRenderer rend = layerGo.AddComponent<SpriteRenderer>();
                bool done = false;
                Sprite loaded = null;
                if (texturePaths[i] != "")
                {
                    yield return StreamingSpriteLoader.LoadSprite(texturePaths[i], true, s => {
                        loaded = s;
                        done = true;
                    });
                    if (!done || loaded == null)
                    {
                        Destroy(layerGo);
                        continue;
                    }
                }
                rend.sprite = loaded;
                layerGo.transform.SetParent(LayersAnchor, false);
                layerGo.transform.localPosition = Vector3.zero;
                if (layerGo) Layers.Add(layerGo);
            }
            var baseRenderer = GetComponent<SpriteRenderer>();
            if (baseRenderer) baseRenderer.forceRenderingOff = true;
            SetColliderSize();
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsComplexCollision) return;
            Bounce(collision);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsComplexCollision) return;
            if (this is not Entity) return; 
            IScript script = other.GetComponent<IScript>();
            script?.Execute(this as Entity);
        }
        private void Bounce(Collision2D collision)
        {
            Rigidbody2D otherRb = collision.rigidbody;
            if (otherRb == null) return;
            if (collision.gameObject.layer != LayerMask.NameToLayer(Type) &&
                collision.gameObject.layer != LayerMask.NameToLayer("Markers"))
            {
                Speed = 0;
                return;
            }
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 pushDirection = (rb.position - otherRb.position).normalized;
            float totalMass = rb.mass + otherRb.mass; // Общая масса двух объектов
            float impulse = CurrentSpeed * rb.mass;  // Импульс игрока

            // Передаем часть импульса другому объекту
            otherRb.AddForce(-pushDirection * (impulse * (rb.mass / totalMass)), ForceMode2D.Impulse);

            //Теряет скорость пропорционально массе другого объекта
            CurrentSpeed *= otherRb.mass / totalMass;
        }
    }
}
