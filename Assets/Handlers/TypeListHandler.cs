using System;
using System.Collections.Generic;

namespace Assets.Handlers
{
    public static class TypeListHandler
    {
        public static readonly Dictionary<string, string[]> equipTypesDict = new(StringComparer.OrdinalIgnoreCase)
        {
            { "turret", new[] { "machine gun", "laser", "light cannon", "cannon", "missile" } },
            { "aircraft", new[] { "fighter", "bomber", "helicopter" } },
            { "launch", new[] { "torpedo" } }
        };

        public static readonly Dictionary<string, string[]> shipTypesDict = new(StringComparer.OrdinalIgnoreCase)
        {
            { "ship", new[] { "boat", "destroyer", "light cruiser", "cruiser", "heavy cruiser", "battleship", "super battleship" } },
            { "aircraft carrier", new[] { "aircraft carrier", "helicopter carrier", "light aircraft carrier", "super aircraft carrier" } },
            { "submarine", new[] { "submarine", "submarine cruiser", "nuclear submarine", "submarine battleship", "submarine aircraft carrier" } }
        };

        public static readonly HashSet<string> sizeTypes = new(StringComparer.OrdinalIgnoreCase) { "s", "m", "l", "xl", "xxl", "x" };
        public static readonly HashSet<string> layerTypes = new(StringComparer.OrdinalIgnoreCase) { "sea", "land", "air" };

        private static readonly Dictionary<string, string> _equipReverseDict = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, string> _shipReverseDict = new(StringComparer.OrdinalIgnoreCase);

        static TypeListHandler()
        {
            InitializeReverseLookup(equipTypesDict, _equipReverseDict);
            InitializeReverseLookup(shipTypesDict, _shipReverseDict);
        }

        private static void InitializeReverseLookup(Dictionary<string, string[]> source, Dictionary<string, string> destination)
        {
            foreach (var kvp in source)
                foreach (var subType in kvp.Value)
                    destination[subType] = kvp.Key;
        }

        public static bool IsWeaponEquipment(string subType) => _equipReverseDict.ContainsKey(subType);

        public static bool IsShip(string subType) => _shipReverseDict.ContainsKey(subType);

        public static string[] TryGetEquipSubTypes(string masterType)
        {
            return equipTypesDict.TryGetValue(masterType, out var subTypes) ? subTypes : Array.Empty<string>();
        }

        public static string[] TryGetShipSubTypes(string masterType)
        {
            return shipTypesDict.TryGetValue(masterType, out var subTypes) ? subTypes : Array.Empty<string>();
        }

        public static string TryGetMasterType(string subType)
        {
            if (_equipReverseDict.TryGetValue(subType, out var weaponMaster)) return weaponMaster;
            if (_shipReverseDict.TryGetValue(subType, out var shipMaster)) return shipMaster;
            return null;
        }
    }
}