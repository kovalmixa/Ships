using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Inventory
{
    public class InventoryItemUI : VisualElement
    {
        [UxmlAttribute]
        private readonly Image _iconImage;
        private readonly Label _countLabel;

        public InventoryItemData AssociatedItem { get; private set; }

        public InventoryItemUI(InventoryItemData itemData, int count)
        {
            var visualTree = Resources.Load<VisualTreeAsset>("InventoryItem");
            visualTree.CloneTree(this);

            _iconImage = this.Q<Image>("Icon");
            _countLabel = this.Q<Label>("Count");

            UpdateData(itemData, count);
        }

        public void UpdateData(InventoryItemData itemData, int count)
        {
            AssociatedItem = itemData;
            _iconImage.image = itemData.general.icon;
            _countLabel.text = count > 1 ? count.ToString() : string.Empty;
        }
    }
}