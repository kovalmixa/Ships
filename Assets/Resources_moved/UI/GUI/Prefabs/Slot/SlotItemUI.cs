using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using Assets.Resources_moved.UI.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class SlotItemUI : VisualElement
    {
        #region Fields

        [UxmlAttribute]
        protected Image iconImage;
        protected Image subImage;
        protected Label numberLabel;

        protected bool showZero;
        protected Vector2 initialSize;
        protected SlotItemData itemData;

        protected VisualElement rootElement;
        public VisualElement Root => rootElement;

        public virtual SlotItemData ItemData
        {
            get => itemData;
            set
            {
                itemData = value;
                if (itemData != null) Debug.Log(itemData.general.nameId);
                if (iconImage != null) iconImage.image = itemData == null ? null : itemData.general.icon;
                else Debug.LogWarning("No image node in slot item ui");
            }
        }

        protected int number;
        public int Number
        {
            get => number;
            set
            {
                number = value;
                if (numberLabel != null)
                    numberLabel.text = number == 0 && !showZero ? string.Empty : number.ToString();
            }
        }

        public Texture2D SubImageTexture
        {
            get
            {
                if (subImage == null) return null;
                return (Texture2D)subImage.image;
            }
            set
            {
                if (subImage != null) subImage.image = value;
            }
        }
    
        public Vector2 Size
        {
            get
            {
                if (rootElement == null) return Vector2.zero;

                float width = float.IsNaN(rootElement.resolvedStyle.width) ? 0f : rootElement.resolvedStyle.width;
                float height = float.IsNaN(rootElement.resolvedStyle.height) ? 0f : rootElement.resolvedStyle.height;

                if (width == 0f && rootElement.style.width.value.unit == LengthUnit.Pixel)
                    width = rootElement.style.width.value.value;

                if (height == 0f && rootElement.style.height.value.unit == LengthUnit.Pixel)
                    height = rootElement.style.height.value.value;

                return new Vector2(width, height);
            }
            set
            {
                if (rootElement == null) return;
                rootElement.style.width = value.x;
                rootElement.style.height = value.y;
            }
        }
        #endregion

        public SlotItemUI(VisualElement root, bool showZero = false)
        {
            this.showZero = showZero;
            rootElement = root;

            VisualElementHandler.SetPickingModeRecursive(rootElement, PickingMode.Ignore);
            if (rootElement != null) rootElement.style.position = Position.Absolute;

            iconImage = root.Q<Image>("IconImage");
            subImage = root.Q<Image>("SubImage");
            numberLabel = root.Q<Label>("NumberLabel");

            if (iconImage == null) Debug.LogWarning("IconImage не найден в SlotItem!");
        }

        #region Size and Position Methods

        public void SetPosition(float x, float y)
        {
            if (rootElement == null) return;
            rootElement.style.left = x;
            rootElement.style.top = y;
        }
        #endregion
    }
}