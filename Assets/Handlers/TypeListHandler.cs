using Assets.Entity.Equipment;
using Assets.Entity.StatMods;
using Assets.Handlers.Enums;
using GameplayActions;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Handlers.Enums
{
    public enum AbilityType
    {
        None,
        FireWeapon,
        LaunchAircraft, LaunchMissile, DropBomb, FireLaser, LaunchTorpedo, SummonDrone,
        Heal, Regeneration, Shield, RadarPulse, Smoke, Dash, Teleport, Repair, Explosion,
        AllTurrets
    }

    public enum AbilityActivationMode
    {
        WeaponGroup, // Activated along with the weapon group (Primary/Secondary/Tertiary)
        ActiveAbility, // Activated by a separate button (ship skill, such as Radar or Dash)
        AutoCast, // Air defense, auto-turrets, etc.
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

        Bullet,
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

    public enum SortingLayerType
    {
        Default = 0,
        Underwater_Terrain,
        Underwater_Decoration,
        Underwater_Decals,
        Underwater_Entities,
        Underwater_Effects,
        Water,
        Water_Decals,
        Water_Effects,
        Ground_Terrain,
        Ground_Decals,
        Ground_Decoration_Bottom,
        Ground_Entities,
        Ground_Effects,
        Ground_Decoration_Top,
        Air_Entities,
        Air_Effects,
        Scripts,
        GameUI,
        PlayerUI
    }

    [Flags]
    public enum InterractLayerType
    {
        None = 0,
        UnderWater = 1 << 0,                // 1
        Water = 1 << 2,                     // 2
        Land = 1 << 3,                      // 4
        Hover = 1 << 4,                     // 8
        Air = 1 << 5,                       // 16

        WaterLayer = UnderWater | Water,    // 5
        GroundLayer = Water | Land | Hover, // 14
        All = WaterLayer | GroundLayer | Air// 31
    }
}

namespace Assets.Handlers
{
    public static class EquipmentHandler
    {
        public static bool IsWeaponEquipment(EquipmentType type) =>
            type == EquipmentType.Turret || type == EquipmentType.Aircraft;

        public static bool IsWeaponEquipment(Equipment equipment) =>
            equipment != null && IsWeaponEquipment((equipment.Data as EquipmentDataSO).type);

        public static Dictionary<WeaponType, SizeType[]> GetWeaponTiers(IEnumerable<Equipment> equipments)
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
                .Where(e => e.Data.General.SizeType != SizeType.None)
                .Select(e => e.Data.General.SizeType)
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

        public static Dictionary<WeaponType, List<Equipment>> GroupWeaponsByTier(IEnumerable<Equipment> equipments)
        {
            var result = new Dictionary<WeaponType, List<Equipment>>
            {
                { WeaponType.Primary, new List<Equipment>() },
                { WeaponType.Secondary, new List<Equipment>() },
                { WeaponType.Tertiary, new List<Equipment>() }
            };

            if (equipments == null) return result;
            var tierSizes = GetWeaponTiers(equipments);
            var weapons = equipments.Where(IsWeaponEquipment).ToList();

            foreach (var weapon in weapons)
            {
                if (weapon?.Data?.General == null) continue;

                var size = weapon.Data.General.SizeType;

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
            { EquipmentType.Turret, new[] { AbilityType.FireWeapon } },
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
            { ProjectileType.Flame, new[] { AbilityType.FireWeapon } },
            { ProjectileType.Gas, new[] { AbilityType.Smoke } }
        };

        public static AbilityType[] GetAbilities(EquipmentDataSO container)
        {
            if (container == null) return Array.Empty<AbilityType>();
            return GetAbilities(container.type, container.projectileType);
        }

        public static AbilityType[] GetAbilities(EquipmentType equipmentType, ProjectileType projectileType)
        {
            EquipmentAbilities.TryGetValue(equipmentType, out var equipAbilities);
            ProjectileAbilities.TryGetValue(projectileType, out var projAbilities);

            equipAbilities ??= Array.Empty<AbilityType>();
            projAbilities ??= Array.Empty<AbilityType>();

            if (equipAbilities.Length == 0 && projAbilities.Length == 0) return Array.Empty<AbilityType>();
            return equipAbilities.Union(projAbilities).ToArray();
        }
    }

    public static class StatModHandler
    {
        public static readonly (DamageType type, StatType dmg, StatType critC, StatType critM, StatType res)[] elementalMap =
        {
        (DamageType.Physical, StatType.PhysicalDamage, StatType.CritChance, StatType.CritMultiplier, StatType.PhysicalResistance),
        (DamageType.Fire, StatType.FireDamage, StatType.FireCritChance, StatType.FireCritMultiplier, StatType.FireResistance),
        (DamageType.Explosive, StatType.ExplosiveDamage, StatType.ExplosiveCritChance, StatType.ExplosiveCritMultiplier, StatType.ExplosiveResistance),
        (DamageType.Acid, StatType.AcidDamage, StatType.AcidCritChance, StatType.AcidCritMultiplier, StatType.AcidResistance),
        (DamageType.Ultrasound, StatType.UltrasoundDamage, StatType.UltrasoundCritChance, StatType.UltrasoundCritMultiplier, StatType.UltrasoundResistance),
        (DamageType.Electricity, StatType.ElectricityDamage, StatType.ElectricityCritChance, StatType.ElectricityCritMultiplier, StatType.ElectricityResistance),
        (DamageType.Plasma, StatType.PlasmaDamage, StatType.PlasmaCritChance, StatType.PlasmaCritMultiplier, StatType.PlasmaResistance),
        (DamageType.Slow, StatType.SlowDamage, StatType.SlowCritChance, StatType.SlowCritMultiplier, StatType.SlowResistance),
        (DamageType.Freeze, StatType.FreezeDamage, StatType.FreezeCritChance, StatType.FreezeCritMultiplier, StatType.FreezeResistance),
        (DamageType.Psi, StatType.PsiDamage, StatType.PsiCritChance, StatType.PsiCritMultiplier, StatType.PsiResistance),
        (DamageType.Radiation, StatType.RadiationDamage, StatType.RadiationCritChance, StatType.RadiationCritMultiplier, StatType.RadiationResistance),
        (DamageType.EMP, StatType.EMPDamage, StatType.EMPCritChance, StatType.EMPCritMultiplier, StatType.EMPResistance),
        (DamageType.SpatialAnomaly, StatType.SpatialAnomalyDamage, StatType.SpatialAnomalyCritChance, StatType.SpatialAnomalyCritMultiplier, StatType.SpatialAnomalyResistance),
        (DamageType.Flooding, StatType.FloodingDamage, StatType.FloodingCritChance, StatType.FloodingCritMultiplier, StatType.FloodingResistance)
    };

        public static StatType GetResistanceStat(DamageType damageType)
        {
            for (int i = 0; i < elementalMap.Length; i++)
                if (elementalMap[i].type == damageType)
                    return elementalMap[i].res;
            return StatType.PhysicalResistance;
        }

        public static float CalculateCritRatio(float critChance, float critMult)
        {
            int elementCritQuantity = (int)critChance;
            if (UnityEngine.Random.value < (critChance - elementCritQuantity)) elementCritQuantity++;
            float elementCrit = elementCritQuantity * critMult;
            return elementCrit;
        }
    }

    public static class LayersHandler
    {
        public static string[] interactionIgnore = { "Markers", "InvisibleMarkers" };

        private static readonly Dictionary<SortingLayerType, int> _layerIdCache = new();

        static LayersHandler()
        {
            foreach (SortingLayerType layer in Enum.GetValues(typeof(SortingLayerType)))
                _layerIdCache[layer] = SortingLayer.NameToID(layer.ToString());
        }

        public static int GetID(SortingLayerType layer)
        {
            return _layerIdCache.TryGetValue(layer, out int id) ? id : 0;
        }

        public static void SetSortingLayer(SpriteRenderer renderer, SortingLayerType layer)
        {
            if (renderer != null) renderer.sortingLayerID = GetID(layer);
        }

        public static int GetPhysicsLayerMask(InterractLayerType targetLayer)
        {
            int mask = 0;
            if ((targetLayer & InterractLayerType.Land) != 0) mask |= LayerMask.GetMask("Land");
            if ((targetLayer & InterractLayerType.Water) != 0) mask |= LayerMask.GetMask("Water");
            if ((targetLayer & InterractLayerType.Air) != 0) mask |= LayerMask.GetMask("Air");
            return mask;
        }
    }
}