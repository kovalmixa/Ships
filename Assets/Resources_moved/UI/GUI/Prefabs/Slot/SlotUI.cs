using Assets.Global.Math;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot
{
    public class SlotStyle
    {
        public Color slotBackgroundColor = new(0f, 0f, 0f, 0.4f);
        public float slotSize = 64;
        public float slotMargin = 2f;

        public float TotalSlotWidth => slotSize + slotMargin * 2;
    }

    public class SlotUI
    {
        protected VisualElement rootElement;
        protected Image backgroundImage;
        protected SlotItemData slotItemData = new();
        public SlotItemData SlotItemData => slotItemData;
        protected SlotItemUI slotItemUI;

        public static SlotStyle SlotStyle => new SlotStyle();

        public SlotUI(VisualElement root)
        {
            rootElement = root;
            backgroundImage = rootElement.Q<Image>("BackgroundImage");
            var slotItemVe = rootElement.Q<VisualElement>("SlotItem");
            if (slotItemVe != null) slotItemUI = new SlotItemUI(slotItemVe);
            else Debug.LogWarning("SlotItem не найден в корне слота!");

            SetupEnterEvent();
            SetupLeaveEvent();
            SetupPointerDownEvent();
            SetupClickPointerUpEvent();
        }

        protected virtual void SetupEnterEvent()
        {
            if (backgroundImage == null) return;
            Color enterColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 0.7f);
            backgroundImage.RegisterCallback<PointerEnterEvent>(evt => backgroundImage.style.backgroundColor = enterColor);
        }

        protected virtual void SetupLeaveEvent()
        {
            if (backgroundImage == null) return;
            backgroundImage.RegisterCallback<PointerLeaveEvent>(evt => backgroundImage.style.backgroundColor = SlotStyle.slotBackgroundColor);
        }

        protected virtual void SetupPointerDownEvent()
        {
            if (backgroundImage == null) return;
            Color clickColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 1f);
            backgroundImage.RegisterCallback<PointerDownEvent>(evt => backgroundImage.style.backgroundColor = clickColor);
        }

        protected virtual void SetupClickPointerUpEvent()
        {
            if (backgroundImage == null) return;
            Color enterColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 0.7f);
            backgroundImage.RegisterCallback<PointerUpEvent>(evt => backgroundImage.style.backgroundColor = enterColor);
        }

        public virtual void SetItem(SlotItemData newItemData)
        {
            this.slotItemData = newItemData;
            if (slotItemUI != null)
            {
                slotItemUI.ItemData = newItemData;
                if (slotItemUI.ItemData == null) Debug.Log("Data is null");
                //else Debug.Log(slotItemUI.ItemData.general.nameId);
            }
        }

        public virtual bool CompareItems(SlotItemData otherData) => this.slotItemData == otherData;
    }
}