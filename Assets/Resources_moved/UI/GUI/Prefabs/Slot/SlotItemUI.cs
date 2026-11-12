using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
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

        public virtual SlotItemData ItemData
        {
            get => itemData;
            set
            {
                itemData = value;
                if (itemData != null)
                    Debug.Log(itemData.general.nameId);

                if (iconImage != null)
                    iconImage.image = itemData == null ? null : itemData.general.icon;
                else
                    Debug.LogWarning("No image node in slot item ui");
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

        #endregion

        public SlotItemUI(VisualElement root, bool showZero = false)
        {
            this.showZero = showZero;
            rootElement = root;

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

        public void SetSize(float width, float height)
        {
            if (rootElement == null) return;
            rootElement.style.width = width;
            rootElement.style.height = height;
        }

        public void CalculateAndSetDimensions(int spanCols, int spanRows, SlotStyle style)
        {
            float cellSize = style.slotSize;
            float margin = style.slotMargin;

            float totalWidth = (spanCols * cellSize) + ((spanCols - 1) * (margin * 2));
            float totalHeight = (spanRows * cellSize) + ((spanRows - 1) * (margin * 2));

            SetSize(totalWidth, totalHeight);
        }

        #endregion
    }
}