using Assets.Handlers.Math;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot {

    public class SlotStyle
    {
        public Color slotBackgroundColor = new(0f, 0f, 0f, 0.4f);
        public float slotSize = 64;
        public float slotMargin = 2f;
    }

    public class SlotUI
    {
        public static SlotStyle SlotStyle => new SlotStyle();

        protected VisualElement visualElement;
        protected SlotItemData slotItemData = new();
        public SlotItemData SlotItemData => slotItemData;

        #region Setup

        public virtual void Setup(VisualElement ve)
        {
            visualElement = ve;

            SetupEnterEvent();
            SetupLeaveEvent();
            SetupPointerDownEvent();
            SetupClickPointerUpEventEvent();
        }

        protected virtual void SetupEnterEvent()
        {
            Color enterColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 0.7f);
            visualElement.RegisterCallback<PointerEnterEvent>(evt => visualElement.style.backgroundColor = enterColor);
        }

        protected virtual void SetupLeaveEvent()
        {
            visualElement.RegisterCallback<PointerLeaveEvent>(evt => visualElement.style.backgroundColor = SlotStyle.slotBackgroundColor);
        }

        protected virtual void SetupPointerDownEvent()
        {
            
            Color clickColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 1f);
            visualElement.RegisterCallback<PointerDownEvent>(evt => visualElement.style.backgroundColor = clickColor);
        }

        protected virtual void SetupClickPointerUpEventEvent()
        {
            Color enterColor = ColorHandler.LightUpColor(SlotStyle.slotBackgroundColor, 0.7f);
            visualElement.RegisterCallback<PointerUpEvent>(evt => visualElement.style.backgroundColor = enterColor);
        }
        #endregion

        #region API

        public virtual void SetItem(SlotItemData slotItemData)
        {
            this.slotItemData = slotItemData;
        }

        public virtual bool CompareItems(SlotItemData slotItemData) => this.slotItemData == slotItemData;
        #endregion
    }
}