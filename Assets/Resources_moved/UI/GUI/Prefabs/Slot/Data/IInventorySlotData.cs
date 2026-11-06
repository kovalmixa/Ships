using Assets.Common.Interfaces;
using Assets.Entity.StatMods;
using Assets.Resources_moved.Common.ContainerComponents;
using System.Collections.Generic;

namespace Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data
{
    public interface IInventorySlotData : IGeneralDataContainer
    {
        InventoryOptions InventoryOptions { get; }
        List<ModUnit> Mods { get; }
    }
}
