using UnityEngine;
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

        protected float Speed { get; set; }
        protected float CurrentSpeed { get; set; }

        protected bool IsTrigger = false;

        public Vector3 Size
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }
        public Vector3 CollisionSize
        {
            get
            {
                var collider = GetComponent<BoxCollider2D>();
                if (collider == null) return Vector3.zero;
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

        protected GameObject Body;

        public void SetRenderLayerOrder(int value)
        {
            foreach (Transform child in transform)
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer == null) continue;
                spriteRenderer.sortingOrder += value;
            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Bounce(collision);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (this is not Hull) return; 
            IScript script = other.GetComponent<IScript>();
            script?.Execute(this as Hull);
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
