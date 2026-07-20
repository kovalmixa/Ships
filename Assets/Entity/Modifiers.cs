using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Entity.Modifiers
{
    public enum StatLayer { Hull, Equipment, Projectile, Global }

    public enum StatCalcType { Set = 0, Addition = 1, Percentage = 2 }

    public enum StatType
    {
        // ==========================================
        // 1. БАЗОВЫЕ И РЕСУРСЫ (0 - 49)
        // ==========================================
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
        ShieldDelay = 10, // Задержка перед началом восстановления щита

        // Люфт 11-49 под будущие ресурсы (например, Ярость, Мана, Стамина)


        // ==========================================
        // 2. ДВИЖЕНИЕ И ФИЗИКА (50 - 99)
        // ==========================================
        MaxMoveSpeed = 50,
        RotationSpeed = 51,
        Mass = 52,
        Acceleration = 53,
        KnockbackResist = 54, // Сопротивление отбрасыванию
        CollisionDamageReduction = 55, // Снижение урона от столкновений/тарана

        // Люфт 56-99 (например, Дальность рывка, Скорость каста рывка)


        // ==========================================
        // 3. БОЕВЫЕ ОБЩИЕ / АТАКА (100 - 149)
        // ==========================================
        Damage = 100, // Базовый общий урон
        DamageMultiplier = 101, // Множитель всего урона (%)
        FireRate = 102, // Скорость атаки / стрельбы
        Penetration = 103, // Пробитие брони (Armor Penetration)
        ElementalChance = 104, // Общий шанс наложения стихийного эффекта
        CritChance = 105, // Общий шанс крита
        CritMultiplier = 106, // Урон крита (например, 150%)
        MaxRange = 107, // Дальность атаки / обзора
        AreaOfEffect = 108, // Радиус взрыва / поражения (AoE)
        CooldownReduction = 109, // Перезарядка способностей (%)
        Vampirism = 110, // Вампиризм / Похищение здоровья (%)
        LifeStealOnKill = 111, // Здоровье за убийство
        EnergyOnKill = 112, // Энергия за убийство

        // Люфт 113-149


        // ==========================================
        // 4. ЗАЩИТА И БРОНЯ (150 - 199)
        // ==========================================
        Armor = 150, // Базовая физическая броня
        Evasion = 151, // Уклонение (%)
        BlockChance = 152, // Шанс блока
        DamageReflection = 153, // Отражение урона (Шипы)

        // Люфт 154-199


        // ==========================================
        // 5. ТИПЫ УРОНА СТИХИЙ (200 - 249)
        // ==========================================
        PhysicalDamage = 200,
        FireDamage = 201, // Огонь
        ExplosiveDamage = 202, // Взрыв
        AcidDamage = 203, // Кислота
        UltrasoundDamage = 204, // Ультразвук
        ElectricityDamage = 205, // Электричество
        PlasmaDamage = 206, // Плазма
        SlowDamage = 207, // Замедление (если наносит урон)
        FreezeDamage = 208, // Заморозка
        PsiDamage = 209, // Пси
        RadiationDamage = 210, // Радиация
        EMPDamage = 211, // Электромагнетизм
        SpatialAnomalyDamage = 212, // Пространственная аномалия
        FloodingDamage = 213, // Затопление

        // Люфт 214-249


        // ==========================================
        // 6. СОПРОТИВЛЕНИЯ / РЕСИСТЫ (250 - 299)
        // ==========================================
        PhysicalResistance = 250,
        FireResistance = 251, // Огнейстойкость
        ExplosiveResistance = 252, // Взрывоустойчивость
        AcidResistance = 253, // Кислотостойкость
        UltrasoundResistance = 254, // Ультразвукоустойчивость
        ElectricityResistance = 255, // Сопротивление электричеству
        PlasmaResistance = 256, // Сопротивление плазме
        SlowResistance = 257, // Сопротивление замедлению
        FreezeResistance = 258, // Сопротивление заморозке / Оглушению
        PsiResistance = 259, // Пси-защита
        RadiationResistance = 260, // Сопротивление радиации
        EMPResistance = 261, // Электромагнитная защита
        SpatialAnomalyResistance = 262, // Устойчивость к аномалиям
        FloodingResistance = 263, // Защита от затопления / гидроизоляция

        // Уязвимость / Дебаффы к стихиям (Взрывоопасность и т.д.)
        ExplosiveVulnerability = 270, // Взрывоопасность (увеличивает входящий урон от взрывов)
        FireVulnerability = 271, // Легковоспламеняемость
        ConductivityVulnerability = 272, // Электропроводность (уязвимость к току)

        // Люфт 273-299


        // ==========================================
        // 7. КРИТИЧЕСКИЙ УРОН И ШАНС КРИТА СТИХИЙ (300 - 349)
        // ==========================================
        // Шансы крита по стихиям
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

        // Множители крита по стихиям (Крит. урон)
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

        // Люфт 333-349


        // ==========================================
        // 8. ДАТА / ПРАМЕТРЫ СНАРЯДОВ (350 - 399)
        // ==========================================
        PrSpeed = 350,
        PrMoveType = 351,
        PrIsHoming = 352,
        PrLifeTime = 353,
        PrPiercingCount = 354, // Сколько врагов насквозь пробивает снаряд
        PrRicochetCount = 355, // Количество отскоков от стен/врагов
        PrCount = 356, // Количество вылетающих снарядов за выстрел (Multishot)
        PrSpreadAngle = 357, // Разброс снарядов

        // Люфт 358-399


        // ==========================================
        // 9. DIABLO-LIKE / НАГРАДЫ / ЭКОНОМИКА (400 - 449)
        // ==========================================
        ItemFind = 400, // Magic Find / Шанс выпадения редкого лута (%)
        GoldFind = 401, // Избыток добычи ресурсов / кредитов (%)
        ExpGain = 402, // Бонус к получаемому опыту (%)
        PickupRadius = 403, // Радиус подбора предметов/сфер
        BuildingSpeed = 404, // Скорость постройки / ремонта (для построек/турелей)
        RepairEfficiency = 405  // Эффективность ремонта (%)

        // Люфт 406-500+
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