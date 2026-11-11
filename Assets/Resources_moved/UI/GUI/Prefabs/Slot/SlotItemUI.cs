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

        #endregion

        public SlotItemUI(VisualElement root, bool showZero = false)
        {
            this.showZero = showZero;

            iconImage = root.Q<Image>("IconImage");
            subImage = root.Q<Image>("SubImage");
            numberLabel = root.Q<Label>("NumberLabel");
            //initialSize = new Vector2(root.resolvedStyle.width, root.resolvedStyle.height);
            //Debug.Log(initialSize);
            if (iconImage == null) Debug.LogWarning("IconImage не найден в SlotItem!");
        }
    }
}