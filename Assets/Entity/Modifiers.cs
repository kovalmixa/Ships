using GameplayActions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Modifiers
{
    public enum StatLayer { Hull, Equipment, Projectile, Global }

    public enum StatCalcType { Set = 0, Addition = 1, Percentage = 2 }

    public enum StatType
    {
        // ==============================================
        // 1. BASIC AND RESOURCES (0 - 49)
        // =============================================
        Hp = 0,
        MaxHp = 1,
        HpRegen = 2,
        Energy = 3,
        MaxEnergy = 4,
        EnergyRegen = 5,
        EnergyPercent = 6,
        Shield = 7,
        MaxShield = 8,
        ShieldRegen = 9,
        ShieldDelay = 10, // Delay before shield recovery starts


        // ==============================================
        // 2. MOVEMENT AND PHYSICS (50 - 99)
        // ==============================================
        MaxMoveSpeed = 50,
        RotationSpeed = 51,
        Mass = 52,
        Acceleration = 53,
        KnockbackResist = 54, // Knockback Resistance
        CollisionDamageReduction = 55, // Damage Reduction from Collisions/Rambling


        // ==============================================
        // 3. COMBAT GENERAL / ATTACK (100 - 149)
        // ==============================================
        Damage = 100, // Base total damage
        DamageMultiplier = 101, // Total damage multiplier (%)
        FireRate = 102, // Attack / Fire Speed
        Penetration = 103, // Armor Penetration
        ElementalChance = 104, // Total chance of applying an elemental effect
        CritChance = 105, // Total critical hit chance
        CritMultiplier = 106, // Critical strike damage (e.g., 150%)
        MaxRange = 107, // Attack/vision range
        AreaOfEffect = 108, // Explosion/damage radius (AoE)
        CooldownReduction = 109, // Ability cooldown (%)
        HPSteal = 110, // Health steal (%)
        EnergySteal = 111, // Energy steal (%)
        ShieldSteal = 112, // Shield steal (%)
        LifeStealOnKill = 113, // Health per kill
        EnergyOnKill = 114, // Energy per kill
        ShieldOnKill = 115, // Shield per kill
        ShootDispersion = 116, // Accuracy of shooting
        Heal = 117, // Heal from projectile to target or by item to host
        DamageLayer = 118, //Damage layers by index of enum of it
        
        // ==============================================
        // 4. PROTECTION AND ARMOR (150 - 199)
        // =============================================
        Armor = 150, // Base Physical Armor
        Evasion = 151, // Evasion (%)
        BlockChance = 152, // Block Chance
        DamageReflection = 153, // Damage Reflection (Spikes)


        // ==============================================
        // 5. ELEMENTAL DAMAGE TYPES (200 - 249)
        // =============================================
        PhysicalDamage = 200,
        FireDamage = 201, // Fire
        ExplosiveDamage = 202, // Explosion
        AcidDamage = 203, // Acid
        UltrasoundDamage = 204, // Ultrasound
        ElectricityDamage = 205, // Electricity
        PlasmaDamage = 206, // Plasma
        SlowDamage = 207, // Slow (if inflicts Damage)
        FreezeDamage = 208, // Freeze
        PsiDamage = 209, // Psi
        RadiationDamage = 210, // Radiation
        EMPDamage = 211, // Electromagnetism
        SpatialAnomalyDamage = 212, // Spatial Anomaly
        FloodingDamage = 213, // Flooding

        // ==================================================
        // 6. RESISTANCES (250 - 299)
        // =================================================
        PhysicalResistance = 250,
        FireResistance = 251, // Fire Resistance
        ExplosiveResistance = 252, // Explosion Resistance
        AcidResistance = 253, // Acid Resistance
        UltrasoundResistance = 254, // Ultrasound Resistance
        ElectricityResistance = 255, // Electricity Resistance
        PlasmaResistance = 256, // Plasma Resistance
        SlowResistance = 257, // Slow Resistance
        FreezeResistance = 258, // Freeze/Stun Resistance
        PsiResistance = 259, // Psi Protection
        RadiationResistance = 260, // Radiation Resistance
        EMPResistance = 261, // Electromagnetic Protection
        SpatialAnomalyResistance = 262, // Anomaly Resistance
        FloodingResistance = 263, // Flooding Protection/Waterproofing
        ShockResistance = 264, // Shock Protection
        StealResistance = 265, // HP/Energy/Shield Steal Protection

        // ===============================================
        // 7. CRITICAL DAMAGE AND ELEMENTAL CRIT CHANCE (300 - 349)
        // ==================================================
        FireCritChance = 300,
        ExplosiveCritChance = 301,
        AcidCritChance = 302,
        UltrasoundCritChance = 303,
        ElectricityCritChance = 304,
        PlasmaCritChance = 305,
        SlowCritChance = 306,
        FreezeCritChance = 307,
        PsiCritChance = 308,
        RadiationCritChance = 309,
        EMPCritChance = 310,
        SpatialAnomalyCritChance = 311,
        FloodingCritChance = 312,
        ShockChance = 313,
        StealChance = 314,

        // Elemental Crit Multipliers (Crit Damage) 
        FireCritMultiplier = 320,
        ExplosiveCritMultiplier = 321,
        AcidCritMultiplier = 322,
        UltrasoundCritMultiplier = 323,
        ElectricityCritMultiplier = 324,
        PlasmaCritMultiplier = 325,
        SlowCritMultiplier = 326,
        FreezeCritMultiplier = 327,
        PsiCritMultiplier = 328,
        RadiationCritMultiplier = 329,
        EMPCritMultiplier = 330,
        SpatialAnomalyCritMultiplier = 331,
        FloodingCritMultiplier = 332,
        ShockCritMultiplier = 333,
        StealCritMultiplier = 334,


        // ===================================================
        // 8. DATE / PROJECTILE PARAMETERS (350 - 399)
        // ================================================
        PrSpeed = 350,
        PrMoveType = 351,
        PrIsHoming = 352,
        PrLifeTime = 353,
        PrPiercingCount = 354, // Number of enemies a projectile pierces
        PrRicochetCount = 355, // Number of wall/enemy bounces
        PrCount = 356, // Number of projectiles fired per shot (Multishot)
        PrType = 357, //Type of projectile (0 - Bullet, 1 - Projectile, etc)

        // ===================================================
        // 9. DIABLO-LIKE / REWARDS / ECONOMY (400 - 449)
        // =============================================
        ItemFind = 400, // Magic Find / Rare Loot Chance (%)
        GoldFind = 401, // Resource/Credit Mining Surplus (%)
        ExpGain = 402, // Experience Bonus (%)
        PickupRadius = 403, // Item/Orb Pickup Radius
        BuildingSpeed = 404, // Construction/Repair Speed ​​(for buildings/turrets)
        RepairEfficiency = 405 // Repair Efficiency (%)
    }

    public enum ModApplyType { Single, Multiple }

    [System.Serializable]
    public class StatUnit
    {
        [HideInInspector] public string name;
        [SerializeField] private StatType _type;
        [SerializeField] private StatLayer _statLayer;
        [SerializeField] private float _value;

        public StatType Type => _type;
        public StatLayer StatLayer => _statLayer;
        public float Value => _value;

        public StatUnit() { }

        public StatUnit(StatType type, StatLayer statLayer, float value)
        {
            _type = type;
            _statLayer = statLayer;
            _value = value;
        }
        
        public StatUnit WithValue(float newValue) => new(Type, StatLayer, newValue);

        public void UpdateInspectorName() => name = $"{_type}: {_value}";
    }
    
    [System.Serializable]
    public class ModUnit : StatUnit
    {

        [SerializeField] private StatCalcType _calcType;
        [SerializeField] private ModApplyType _applyType;

        public StatCalcType CalcType => _calcType;
        public ModApplyType ApplyType => _applyType;

        public ModUnit() { }

        public ModUnit(StatType type, StatLayer layer, StatCalcType calcType, ModApplyType applyType, float value)
            : base(type, layer, value)
        {
            _calcType = calcType;
            _applyType = applyType;
        }

        public new ModUnit WithValue(float newValue) => new(Type, StatLayer, CalcType, ApplyType, newValue);
    }

    public class Modifiers
    {
        public class CompactMod
        {
            public float? SetValue { get; set; }
            public float Addition { get; set; }
            public float Percentage { get; set; }
            public ModApplyType ApplyType { get; set; }
            public bool IsEmpty => SetValue == null && Addition == 0f && Percentage == 0f;
        }

        private readonly Dictionary<(StatType Type, StatLayer Layer), CompactMod> _modsMap = new();

        public IEnumerable<ModUnit> StatsMods => _modsMap.SelectMany(kvp =>
        {
            var (type, layer) = kvp.Key;
            var mod = kvp.Value;
            var list = new List<ModUnit>();

            if (mod.SetValue.HasValue) list.Add(new ModUnit(type, layer, StatCalcType.Set, mod.ApplyType, mod.SetValue.Value));
            if (mod.Addition != 0f) list.Add(new ModUnit(type, layer, StatCalcType.Addition, mod.ApplyType, mod.Addition));
            if (mod.Percentage != 0f) list.Add(new ModUnit(type, layer, StatCalcType.Percentage, mod.ApplyType, mod.Percentage));

            return list;
        });

        public Modifiers() { }
        public Modifiers(IEnumerable<ModUnit> modUnits) => Add(modUnits);

        public CompactMod GetMod(StatType type, StatLayer layer)
        {
            _modsMap.TryGetValue((type, layer), out var mod);
            return mod;
        }

        public void Add(IEnumerable<ModUnit> modUnits)
        {
            if (modUnits == null) return;
            foreach (var mod in modUnits) AddSingle(mod);
        }

        public void Add(Modifiers otherModifiers)
        {
            if (otherModifiers == null) return;
            foreach (var incomingMod in otherModifiers.StatsMods) AddSingle(incomingMod);
        }

        public void AddSingle(ModUnit mod)
        {
            var key = (mod.Type, mod.StatLayer);
            if (!_modsMap.TryGetValue(key, out var compactMod))
            {
                compactMod = new CompactMod();
                _modsMap[key] = compactMod;
            }

            switch (mod.CalcType)
            {
                case StatCalcType.Set:
                    compactMod.SetValue = mod.Value;
                    break;
                case StatCalcType.Addition:
                    compactMod.Addition += mod.Value;
                    break;
                case StatCalcType.Percentage:
                    compactMod.Percentage += mod.Value;
                    break;
            }
        }

        public void Clear() => _modsMap.Clear();
    }
}