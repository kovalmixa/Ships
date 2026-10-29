using UnityEngine;
using UnityEngine.UIElements;

public class InventoryItemUI : VisualElement
{
    [UxmlAttribute]
    private readonly Image iconImage;
    private readonly Label countLabel;

    public ItemData AssociatedItem { get; private set; }

    public InventoryItemUI(ItemData itemData, int count)
    {
        var visualTree = Resources.Load<VisualTreeAsset>("InventoryItem");
        visualTree.CloneTree(this);

        iconImage = this.Q<Image>("Icon");
        countLabel = this.Q<Label>("Count");

        UpdateData(itemData, count);
    }

    public void UpdateData(ItemData itemData, int count)
    {
        AssociatedItem = itemData;
        iconImage.image = itemData.icon;
        countLabel.text = count > 1 ? count.ToString() : string.Empty;
    }
}