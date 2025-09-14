using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Handlers
{
    public static class TypeListHandler
    {
        public static readonly Dictionary<string, string[]> WeaponEquipTypesDict = new()
        {
            {
                "turret", new[] { "machine gun", "laser", "light cannon", "cannon", "missile" }
            },
            {
                "aircraft", new[] { "fighter", "bomber", "helicopter" }
            },
            {
                "launch", new[] { "torpedo" }
            }
        };

        public static bool IsWeaponEquipment(string type) => DoesBelongToDictionary(WeaponEquipTypesDict, type);

        public static string[] TryGetSubType(string command)
        {
            foreach (var weapons in WeaponEquipTypesDict)
                if (weapons.Key == command) return weapons.Value;
            return null;
        }

        public static string TryGetMasterType(string subType)
        {
            foreach (var weapons in WeaponEquipTypesDict)
            {
                if (weapons.Value.Contains(subType)) return weapons.Key;
            }
            return null;
        }

        public static readonly Dictionary<string, string[]> ShipTypesDict = new()
        {
            {
                "ship", new[]
                {
                    "boat", "destroyer", "light cruiser", "cruiser", "heavy cruiser", "battleship", "super battleship"
                }
            },
            {
                "aircraft carrier", new[]
                {
                    "aircraft carrier", "helicopter carrier", "light aircraft carrier", "super aircraft carrier"
                }
            },
            {
                "submarine", new[]
                {
                    "submarine", "submarine cruiser", "nuclear submarine", "submarine battleship",
                    "submarine aircraft carrier"
                }
            }
        };

        public static readonly string[] SizeTypes =
        {
            "s", "S", "M", "L", "XL", "XXL", "X"
        };

        public static readonly string[] LayerTypes =
        {
            "sea", "land", "air"
        };

        private static bool DoesBelongToDictionary(Dictionary<string, string[]> dictionary, string type)
        {
            return dictionary.Any(keyValuePair => keyValuePair.Value.ToList().Any(x => x == type));
        }
    }
}
