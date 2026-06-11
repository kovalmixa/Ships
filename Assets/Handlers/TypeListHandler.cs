using System;
using System.Collections.Generic;

namespace Assets.Handlers
{
    public static class TypeListHandler
    {
        public static readonly Dictionary<string, string[]> EquipTypesDict = new(StringComparer.OrdinalIgnoreCase)
        {
            { "turret", new[] { "machine gun", "laser", "light cannon", "cannon", "missile" } },
            { "aircraft", new[] { "fighter", "bomber", "helicopter" } },
            { "launch", new[] { "torpedo" } }
        };

        public static readonly Dictionary<string, string[]> ShipTypesDict = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ship", new[] { "boat", "destroyer", "light cruiser", "cruiser", "heavy cruiser", "battleship", "super battleship" } },
            { "aircraft carrier", new[] { "aircraft carrier", "helicopter carrier", "light aircraft carrier", "super aircraft carrier" } },
            { "submarine", new[] { "submarine", "submarine cruiser", "nuclear submarine", "submarine battleship", "submarine aircraft carrier" } }
        };

        public static readonly HashSet<string> SizeTypes = new(StringComparer.OrdinalIgnoreCase) { "s", "m", "l", "xl", "xxl", "x" };
        public static readonly HashSet<string> LayerTypes = new(StringComparer.OrdinalIgnoreCase) { "sea", "land", "air" };

        private static readonly Dictionary<string, string> EquipReverseDict = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> ShipReverseDict = new(StringComparer.OrdinalIgnoreCase);

        static TypeListHandler()
        {
            InitializeReverseLookup(EquipTypesDict, EquipReverseDict);
            InitializeReverseLookup(ShipTypesDict, ShipReverseDict);
        }

        private static void InitializeReverseLookup(Dictionary<string, string[]> source, Dictionary<string, string> destination)
        {
            foreach (var kvp in source)
                foreach (var subType in kvp.Value)
                    destination[subType] = kvp.Key;
        }

        public static bool IsWeaponEquipment(string subType) => EquipReverseDict.ContainsKey(subType);

        public static bool IsShip(string subType) => ShipReverseDict.ContainsKey(subType);

        public static string[] TryGetEquipSubTypes(string masterType)
        {
            return EquipTypesDict.TryGetValue(masterType, out var subTypes) ? subTypes : Array.Empty<string>();
        }

        public static string[] TryGetShipSubTypes(string masterType)
        {
            return ShipTypesDict.TryGetValue(masterType, out var subTypes) ? subTypes : Array.Empty<string>();
        }

        public static string TryGetMasterType(string subType)
        {
            if (EquipReverseDict.TryGetValue(subType, out var weaponMaster)) return weaponMaster;
            if (ShipReverseDict.TryGetValue(subType, out var shipMaster)) return shipMaster;
            return null;
        }
    }
}