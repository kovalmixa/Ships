using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.UIToolkit
{
    [Serializable]
    public class PaddingVector
    {
        [SerializeField] protected float left;
        [SerializeField] protected float right;
        [SerializeField] protected float top;
        [SerializeField] protected float bottom;

        public float Left => left;
        public float Right => right;
        public float Top => top;
        public float Bottom => bottom;

        protected PaddingVector() { }

        public PaddingVector(float left, float right, float top, float bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        public PaddingVector(VisualElement element)
        {
            left = element.resolvedStyle.paddingLeft;
            right = element.resolvedStyle.paddingRight;
            top = element.resolvedStyle.paddingTop;
            bottom = element.resolvedStyle.paddingBottom;
        }

        #region Conversion to/from Vector4

        public static implicit operator Vector4(PaddingVector p)
        {
            if (p == null) return Vector4.zero;
            return new Vector4(p.left, p.right, p.top, p.bottom);
        }

        public static explicit operator PaddingVector(Vector4 v)
        {
            return new PaddingVector(v.x, v.y, v.z, v.w);
        }

        #endregion

        #region Math

        public static PaddingVector operator +(PaddingVector a, PaddingVector b)
        {
            return new PaddingVector(a.left + b.left, a.right + b.right, a.top + b.top, a.bottom + b.bottom);
        }

        public static PaddingVector operator -(PaddingVector a, PaddingVector b)
        {
            return new PaddingVector(a.left - b.left, a.right - b.right, a.top - b.top, a.bottom - b.bottom);
        }

        public static PaddingVector operator *(PaddingVector padding, float scalar)
        {
            return new PaddingVector(padding.left * scalar, padding.right * scalar, padding.top * scalar, padding.bottom * scalar);
        }

        public static PaddingVector operator *(float scalar, PaddingVector padding)
        {
            return padding * scalar;
        }

        public static PaddingVector operator /(PaddingVector padding, float scalar)
        {
            if (scalar == 0f) throw new DivideByZeroException("Cannot divide PaddingStruct by zero.");
            return new PaddingVector(padding.left / scalar, padding.right / scalar, padding.top / scalar, padding.bottom / scalar);
        }

        public static PaddingVector operator -(PaddingVector padding)
        {
            return new PaddingVector(-padding.left, -padding.right, -padding.top, -padding.bottom);
        }

        #endregion
    }
}