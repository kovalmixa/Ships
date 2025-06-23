using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Handlers
{
    public static class TypeListHandler
    {
        public static readonly Dictionary<string, string[]> WeaponTypesDict = new()
        {
            {
                "fast projectile", new[]{"machine gun", "laser", "light cannon", "cannon"}
            },
            {
                "aircraft", new[]{"fighter", "bomber", "helicopter"}
            },
            {
                "slow projectile", new[]{"torpedo", "missile"}
            }
        };
        public static readonly Dictionary<string, string[]> ShipTypesDict = new()
        {
            {
                "ship", new []
                {
                    "boat", "destroyer", "light cruiser", "cruiser", "heavy cruiser", "battleship", "super battleship"
                }
            },
            {
                "aircraft carrier", new [] 
                {
                    "aircraft carrier", "helicopter carrier", "light aircraft carrier", "super aircraft carrier"
                }
            },
            {
                "submarine", new [] 
                { 
                    "submarine", "submarine cruiser", "nuclear submarine", "submarine battleship", "submarine aircraft carrier"
                }
            }
        };

        public static bool IsWeapon(string type) => DoesBelongToDictionary(WeaponTypesDict, type);
        public static bool IsShip(string type) => DoesBelongToDictionary(ShipTypesDict, type);
        private static bool DoesBelongToDictionary(Dictionary<string, string[]> dictionary, string type)
        {
            foreach (var keyValuePair in dictionary)
            {
                if (keyValuePair.Value.ToList().Any(x => x == type))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
