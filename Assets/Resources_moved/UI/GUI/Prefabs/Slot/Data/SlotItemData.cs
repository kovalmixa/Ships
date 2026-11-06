using Assets.Common.Interfaces;
using Assets.Resources_moved.Common.ContainerComponents;
using System;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data
{
    [Serializable]
    public class SlotItemData : IGeneralDataContainer
    {
        public GeneralOptions general;
        public GeneralOptions General => general;
    }
}
