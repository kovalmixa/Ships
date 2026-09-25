#ifndef OUTLINE_INCLUDED
#define OUTLINE_INCLUDED

void Outline_float(
    float2 UV, 
    float FillAmount, 
    float BorderWidth, 
    float Aspect, // Соотношение сторон объекта (Ширина / Высота)
    float4 BorderColor, 
    float4 FillColor, 
    float4 BgColor, 
    out float4 OutColor)
{
    // 1. Корректируем толщину боковых рамок по оси X в зависимости от пропорций (Aspect)
    // Защита max() гарантирует, что при нулевом Aspect шейдер не сломается
    float safeAspect = max(0.001, Aspect);
    float borderWidthX = BorderWidth / safeAspect;
    float borderWidthY = BorderWidth;

    // 2. Проверяем, находится ли пиксель на рамке (со всех 4 сторон)
    bool isBorder = (UV.x < borderWidthX) || (UV.x > 1.0 - borderWidthX) ||
                    (UV.y < borderWidthY) || (UV.y > 1.0 - borderWidthY);

    // 3. Вычисляем заполнение шкалы СТРОГО во внутреннем пространстве (между рамками)
    float fillUV = (UV.x - borderWidthX) / max(0.0001, (1.0 - 2.0 * borderWidthX));
    bool isFilled = fillUV <= FillAmount;

    // 4. Жесткая приоритетная логика:
    // Рамка ВСЕГДА имеет наивысший приоритет
    if (isBorder) OutColor = BorderColor;
    else if (isFilled) OutColor = FillColor;
    else OutColor = BgColor;
}

#endif