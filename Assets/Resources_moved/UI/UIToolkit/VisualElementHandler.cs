using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.UIToolkit
{
    public static class VisualElementHandler
    {
        public static void SetPickingModeRecursive(VisualElement element, PickingMode mode)
        {
            if (element == null) return;
            element.pickingMode = mode;
            foreach (var child in element.Children()) SetPickingModeRecursive(child, mode);
        }
    }
}
