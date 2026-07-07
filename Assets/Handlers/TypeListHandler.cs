using Assets.Handlers.Enums;
using System;
using System.Collections.Generic;

namespace Assets.Handlers.Enums
{
    public enum AbilityType
    {
        None, AllTurrets,
        FirePrimary, FireSecondary, LaunchAircraft, LaunchMissile,
        DropBomb, FireLaser, LaunchTorpedo,
        Heal, Regeneration, Shield, RadarPulse, Smoke,
        Dash, Teleport, Repair, SummonDrone
    }

    public enum EquipmentMasterType
    {
        None,
        Turret,
        Aircraft,
        Torpedo,
        Missile,
        Engine,
        Radar,
        Shield,
        Utility
    }

    public enum EquipmentSubType
    {
        None,
        MachineGun, FlameGun, Laser, LightCannon, Cannon, Missile,
        Fighter, Bomber, Helicopter,
        Torpedo
    }

    public enum VehicleMasterType
    {
        None,
        Ship,
        AircraftCarrier,
        Submarine
    }

    public enum VehicleSubType
    {
        None,
        Boat, Destroyer, LightCruiser, Cruiser, HeavyCruiser, Battleship, SuperBattleship,
        AircraftCarrier, HelicopterCarrier, LightAircraftCarrier, SuperAircraftCarrier,
        Submarine, SubmarineCruiser, NuclearSubmarine, SubmarineBattleship, SubmarineAircraftCarrier
    }

    public enum ProjectileType
    {
        None, Projectile, Missile, Beam
    }

    public enum SizeType
    {
        None, S, M, L, XL, XXL, X
    }

    public enum LayerType
    {
        None, Sea, Land, Air
    }
}

namespace Assets.Handlers
{
    public static class EquipmentHandler
    {
        public static readonly Dictionary<EquipmentMasterType, EquipmentSubType[]> TypesDict = new()
        {
            { EquipmentMasterType.Turret, new[] {
                EquipmentSubType.MachineGun, EquipmentSubType.FlameGun, EquipmentSubType.Laser,
                EquipmentSubType.LightCannon, EquipmentSubType.Cannon, EquipmentSubType.Missile }
            },
            { EquipmentMasterType.Aircraft, new[] {
                EquipmentSubType.Fighter, EquipmentSubType.Bomber, EquipmentSubType.Helicopter }
            },
            { EquipmentMasterType.Torpedo, new[] {
                EquipmentSubType.Torpedo }
            }, 
            { EquipmentMasterType.Missile, new[] {
                EquipmentSubType.Missile }
            }
        };

        private static readonly Dictionary<EquipmentSubType, EquipmentMasterType> _reverseDict = new();

        static EquipmentHandler()
        {
            foreach (var kvp in TypesDict)
                foreach (var subType in kvp.Value)
                    _reverseDict[subType] = kvp.Key;
        }

        public static bool IsWeaponEquipment(EquipmentSubType subType) => _reverseDict.ContainsKey(subType);

        public static EquipmentSubType[] TryGetSubTypes(EquipmentMasterType masterType) =>
            TypesDict.TryGetValue(masterType, out var subTypes) ? subTypes : Array.Empty<EquipmentSubType>();

        public static EquipmentMasterType TryGetMasterType(EquipmentSubType subType) =>
            _reverseDict.TryGetValue(subType, out var master) ? master : EquipmentMasterType.None;
    }

    public static class VehicleHandler
    {
        public static readonly Dictionary<VehicleMasterType, VehicleSubType[]> TypesDict = new()
        {
            { VehicleMasterType.Ship, new[] {
                VehicleSubType.Boat, VehicleSubType.Destroyer, VehicleSubType.LightCruiser,
                VehicleSubType.Cruiser, VehicleSubType.HeavyCruiser, VehicleSubType.Battleship, VehicleSubType.SuperBattleship }
            },
            { VehicleMasterType.AircraftCarrier, new[] {
                VehicleSubType.AircraftCarrier, VehicleSubType.HelicopterCarrier,
                VehicleSubType.LightAircraftCarrier, VehicleSubType.SuperAircraftCarrier }
            },
            { VehicleMasterType.Submarine, new[] {
                VehicleSubType.Submarine, VehicleSubType.SubmarineCruiser, VehicleSubType.NuclearSubmarine,
                VehicleSubType.SubmarineBattleship, VehicleSubType.SubmarineAircraftCarrier }
            }
        };

        private static readonly Dictionary<VehicleSubType, VehicleMasterType> _reverseDict = new();

        static VehicleHandler()
        {
            foreach (var kvp in TypesDict)
                foreach (var subType in kvp.Value)
                    _reverseDict[subType] = kvp.Key;
        }

        public static bool IsVehicle(VehicleSubType subType) => _reverseDict.ContainsKey(subType);

        public static VehicleSubType[] TryGetSubTypes(VehicleMasterType masterType) =>
            TypesDict.TryGetValue(masterType, out var subTypes) ? subTypes : Array.Empty<VehicleSubType>();

        public static VehicleMasterType TryGetMasterType(VehicleSubType subType) =>
            _reverseDict.TryGetValue(subType, out var master) ? master : VehicleMasterType.None;
    }

    public static class AbilityHandler
    {
        public static readonly Dictionary<EquipmentMasterType, AbilityType[]> MasterTypeAbilities = new()
        {
            { EquipmentMasterType.Turret, new[] { AbilityType.FirePrimary } },
            { EquipmentMasterType.Torpedo, new[] { AbilityType.LaunchMissile } },
            { EquipmentMasterType.Missile, new[] { AbilityType.LaunchMissile } },
            { EquipmentMasterType.Aircraft, new[] { AbilityType.LaunchAircraft } },
            { EquipmentMasterType.Radar, new[] { AbilityType.RadarPulse } },
            { EquipmentMasterType.Shield, new[] { AbilityType.Shield } }
        };

        public static readonly Dictionary<EquipmentSubType, AbilityType[]> SubTypeAbilities = new()
        {
            { EquipmentSubType.Bomber, new[] { AbilityType.DropBomb } },
            { EquipmentSubType.Laser, new[] { AbilityType.FireLaser } },
            { EquipmentSubType.Torpedo, new[] { AbilityType.LaunchTorpedo } }
        };

        public static AbilityType[] GetMasterAbilities(EquipmentMasterType masterType) =>
            MasterTypeAbilities.TryGetValue(masterType, out var abilities) ? abilities : Array.Empty<AbilityType>();

        public static AbilityType[] GetSpecificAbilities(EquipmentSubType subType) =>
            SubTypeAbilities.TryGetValue(subType, out var abilities) ? abilities : Array.Empty<AbilityType>();

        public static AbilityType[] GetAllAbilitiesFor(EquipmentSubType subType)
        {
            var masterType = EquipmentHandler.TryGetMasterType(subType);

            var masterAbilities = GetMasterAbilities(masterType);
            var specificAbilities = GetSpecificAbilities(subType);

            if (masterAbilities.Length == 0 && specificAbilities.Length == 0) return Array.Empty<AbilityType>();

            var combinedAbilities = new HashSet<AbilityType>(masterAbilities);
            foreach (var ability in specificAbilities) combinedAbilities.Add(ability);

            var result = new AbilityType[combinedAbilities.Count];
            combinedAbilities.CopyTo(result);
            return result;
        }
    }
}