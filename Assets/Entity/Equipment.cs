using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Entity
{
    public class Equipment : InGameObject
    {
        public bool CanBeRotatedByControl { get; set; }
        public new string Type { get; set; }

        public void Rotate(Angle angle)
        {
            if (!CanBeRotatedByControl) return;
            //code for rotation
        }

        public void Activate()
        {

        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
        }
    }
}
