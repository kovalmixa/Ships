using UnityEngine;

namespace Assets.Handlers.Math
{
    public static class ColorHandler
    {
        public static Color fallBackColor = new Color(1f, 0.0f, 1f, 1f);
        public static Color LightUpColor(Color baseColor, float ratio)
        {
            Color result = Color.Lerp(baseColor, Color.white, ratio);
            result.a = baseColor.a;
            return result;
        }
    }
}