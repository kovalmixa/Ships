using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.UIToolkit
{
    public class MarginVector : PaddingVector
    {
        public MarginVector(float left, float right, float top, float bottom) : base(left, right, top, bottom) { }

        public MarginVector(VisualElement element)
        {
            left = element.resolvedStyle.marginLeft;
            right = element.resolvedStyle.marginRight;
            top = element.resolvedStyle.marginTop;
            bottom = element.resolvedStyle.marginBottom;
        }
    }
}
