using Assets.Entity.Equipment;
using Assets.Handlers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public enum WeaponType
    {
        None,
        Primary, // Main Caliber (largest blood)
        Secondary, // Second Caliber (medium)
        Tertiary // Third Caliber / Auxiliary Weapon (small)
    }

    public enum EquipmentType
    {
        None,
        Turret,
        Aircraft,
        Engine,
        Radar,
        Shield,
        Utility
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
        Boat, GunBoat, Corvette, Fregate, Destroyer, LightCruiser, Cruiser, HeavyCruiser, Battleship, SuperBattleship,
        AircraftCarrier, HelicopterCarrier, LightAircraftCarrier, SuperAircraftCarrier,
        Submarine, SubmarineCruiser, NuclearSubmarine, SubmarineBattleship, SubmarineAircraftCarrier
    }

    public enum ProjectileType 
    {
        None = 0,

        Projectile, // Standard machine gun/projectile
        Shot, // Shotgun / Volley

        Missile, // Heavy missile
        Swarm, // Micro-missile swarm (Multishot)
        Torpedo, // Underwater torpedo (ignores shields)
        KamikazeDrone, // Boarding pod / Drone

        DepthCharge, // Depth charge (detonates on timer/distance)
        Mine, // Naval booby trap
        AcidContainer, // Acid/gas capsule (creates a DoT zone)
        Bomb,

        Gas, // Gas cloud
        Beam, // Continuous beam (laser/scorcher)
        Plasma, // Plasma bolt
        Flame, // Flamethrower Stream
        ChainLightning, // Tesla Chain Discharge (arc across faces)
        SonicWave, // Ultrasonic Wave through walls
        Vortex // Gravity Vortex Anomaly (pulls enemies in)
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
        /// <summary>
        /// Проверяет, является ли оборудование оружием.
        /// </summary>
        public static bool IsWeaponEquipment(EquipmentType type) =>
            type == EquipmentType.Turret || type == EquipmentType.Aircraft;

        /// <summary>
        /// Проверяет контейнер оборудования на принадлежность к оружию.
        /// </summary>
        public static bool IsWeaponEquipment(EquipmentContainer container) =>
            container != null && IsWeaponEquipment(container.Type);

        /// <summary>
        /// Динамически распределяет имеющиеся размеры оружия по тирам (Primary, Secondary, Tertiary).
        /// </summary>
        /// <param name="equipments">Массив оборудования (например, установленного на корабле).</param>
        /// <returns>Словарь тиров. Если тир отсутствует, его значение равен null.</returns>
        public static Dictionary<WeaponType, SizeType[]> GetWeaponTiers(IEnumerable<EquipmentContainer> equipments)
        {
            var result = new Dictionary<WeaponType, SizeType[]>
            {
                { WeaponType.Primary, null },
                { WeaponType.Secondary, null },
                { WeaponType.Tertiary, null }
            };

            if (equipments == null) return result;

            var availableSizes = equipments
                .Where(IsWeaponEquipment)
                .Where(e => e.general != null && e.general.SizeType != SizeType.None)
                .Select(e => e.general.SizeType)
                .Distinct()
                .OrderByDescending(size => (int)size)
                .ToList();

            if (availableSizes.Count == 0) return result;

            result[WeaponType.Primary] = new[] { availableSizes[0] };

            if (availableSizes.Count == 2) result[WeaponType.Secondary] = new[] { availableSizes[1] };
            else if (availableSizes.Count >= 3)
            {
                result[WeaponType.Secondary] = new[] { availableSizes[1] };
                result[WeaponType.Tertiary] = availableSizes.Skip(2).ToArray();
            }

            return result;
        }

        public static Dictionary<WeaponType, List<EquipmentContainer>> GroupWeaponsByTier(IEnumerable<EquipmentContainer> equipments)
        {
            var tierSizes = GetWeaponTiers(equipments);
            var result = new Dictionary<WeaponType, List<EquipmentContainer>>
            {
                { WeaponType.Primary, new List<EquipmentContainer>() },
                { WeaponType.Secondary, new List<EquipmentContainer>() },
                { WeaponType.Tertiary, new List<EquipmentContainer>() }
            };

            if (equipments == null) return result;

            var weapons = equipments.Where(IsWeaponEquipment).ToList();

            foreach (var weapon in weapons)
            {
                if (weapon.general == null) continue;

                var size = weapon.general.SizeType;

                if (tierSizes[WeaponType.Primary]?.Contains(size) == true)
                    result[WeaponType.Primary].Add(weapon);
                else if (tierSizes[WeaponType.Secondary]?.Contains(size) == true)
                    result[WeaponType.Secondary].Add(weapon);
                else if (tierSizes[WeaponType.Tertiary]?.Contains(size) == true)
                    result[WeaponType.Tertiary].Add(weapon);
            }

            return result;
        }
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
        private static readonly Dictionary<EquipmentType, AbilityType[]> EquipmentAbilities = new()
        {
            { EquipmentType.Turret, new[] { AbilityType.FirePrimary } },
            { EquipmentType.Aircraft, new[] { AbilityType.LaunchAircraft } },
            { EquipmentType.Radar, new[] { AbilityType.RadarPulse } },
            { EquipmentType.Shield, new[] { AbilityType.Shield } },
            { EquipmentType.Engine, new[] { AbilityType.Dash } },
            { EquipmentType.Utility, new[] { AbilityType.Repair } }
        };

        private static readonly Dictionary<ProjectileType, AbilityType[]> ProjectileAbilities = new()
        {
            { ProjectileType.Bomb, new[] { AbilityType.DropBomb } },
            { ProjectileType.Beam, new[] { AbilityType.FireLaser } },
            { ProjectileType.Torpedo, new[] { AbilityType.LaunchTorpedo } },
            { ProjectileType.Missile, new[] { AbilityType.LaunchMissile } },
            { ProjectileType.Swarm, new[] { AbilityType.LaunchMissile } },
            { ProjectileType.KamikazeDrone, new[] { AbilityType.SummonDrone } },
            { ProjectileType.Flame, new[] { AbilityType.FireSecondary } },
            { ProjectileType.Gas, new[] { AbilityType.Smoke } }
        };

        public static AbilityType[] GetAbilities(EquipmentContainer container)
        {
            if (container == null) return Array.Empty<AbilityType>();
            return GetAbilities(container.Type, container.ProjectileType);
        }

        public static AbilityType[] GetAbilities(EquipmentType equipmentType, ProjectileType projectileType)
        {
            EquipmentAbilities.TryGetValue(equipmentType, out var equipAbilities);
            ProjectileAbilities.TryGetValue(projectileType, out var projAbilities);

            equipAbilities ??= Array.Empty<AbilityType>();
            projAbilities ??= Array.Empty<AbilityType>();

            if (equipAbilities.Length == 0 && projAbilities.Length == 0)
                return Array.Empty<AbilityType>();

            return equipAbilities.Union(projAbilities).ToArray();
        }
    }
}