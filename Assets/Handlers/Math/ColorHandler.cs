using UnityEngine;

namespace Assets.Handlers.Math
{
    public static class ColorHandler
    {
        public static Color LightUpColor(Color baseColor, float ratio = 0.15f)
        {
            float h, s, v;
            Color.RGBToHSV(baseColor, out h, out s, out v);

            v = Mathf.Clamp01(v + ratio);
            s = Mathf.Clamp01(s - ratio);

            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = baseColor.a;

            return newColor;
        }
    }
}