using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class SlotItemUI : VisualElement
    {
        #region Fields
        [UxmlAttribute]
        private readonly Image _iconImage;
        private readonly Image _subImage;
        private readonly Label _numberLabel;

        private bool _showZero;

        private SlotItemData _itemData;
        public SlotItemData ItemData 
        { 
            get => _itemData;
            set
            {
                _itemData = value;
                //if (_iconImage != null) _iconImage.image = _itemData.general.icon;
                if (_iconImage != null)
                {
                    Debug.Log("Image node was found");
                    _iconImage.image = _itemData.general.icon;
                }
                else Debug.Log("No image node");
            }
        }

        private int _number;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                if (_numberLabel != null)
                    _numberLabel.text = _number == 0 && !_showZero ? string.Empty : _number.ToString();
            }
        }

        public Texture2D SubImageTexture
        {
            get
            {
                if (_subImage == null) return null;
                return (Texture2D)_subImage.image;
            }
            set
            {
                if (_subImage != null) _subImage.image = value;
            }
        }

        #endregion

        #region

        public SlotItemUI(bool showZero = false)
        {
            _showZero = showZero;

            var visualTree = Resources.Load<VisualTreeAsset>("SlotItem");
            if (visualTree == null)
            {
                Debug.LogWarning("No slot item uxml prefab was found!");
                return;
            }
            visualTree.CloneTree(this);

            _iconImage = this.Q<Image>("IconImage");
            _subImage = this.Q<Image>("SubImage");
            _numberLabel = this.Q<Label>("NumberLabel");
        }

        #endregion
    }
}
