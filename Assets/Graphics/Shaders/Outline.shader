Shader "Custom/ProgressBarWithBorder"
{
    Properties
    {
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.75
        _BorderWidth ("Border Width", Range(0, 0.2)) = 0.05
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _FillColor ("Fill Color", Color) = (0, 0.8, 0.2, 1)
        _BgColor ("Background Color", Color) = (0.15, 0.15, 0.15, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _FillAmount;
            float _BorderWidth;
            fixed4 _BorderColor;
            fixed4 _FillColor;
            fixed4 _BgColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Проверяем, находится ли пиксель на рамке (по всем 4 краям)
                bool isBorder = (uv.x < _BorderWidth) || (uv.x > 1.0 - _BorderWidth) ||
                                (uv.y < _BorderWidth) || (uv.y > 1.0 - _BorderWidth);

                // Пересчитываем UV заполнения шкалы только внутри рамки
                float fillUV = (uv.x - _BorderWidth) / (1.0 - 2.0 * _BorderWidth);
                bool isFilled = fillUV <= _FillAmount;

                if (isBorder)
                    return _BorderColor;
                if (isFilled)
                    return _FillColor;
                
                return _BgColor;
            }
            ENDCG
        }
    }
}