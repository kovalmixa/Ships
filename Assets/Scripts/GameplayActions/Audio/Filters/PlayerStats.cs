using Assets.Entity.Controllers;
using Assets.Entity.Modifiers;
using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.GameplayActions.Audio.Filters
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private StatModController _playerStats;

        private void OnEnable() => _playerStats.OnChange += UpdatePlayerParameter;
        private void OnDisable() => _playerStats.OnChange -= UpdatePlayerParameter;

        private void UpdatePlayerParameter()
        {
            HealthParametr();
        }

        private void HealthParametr()
        {
            float hp = _playerStats.GetStat(StatType.Hp, StatLayer.Global);
            float maxHp = _playerStats.GetStat(StatType.MaxHp, StatLayer.Global);
            float normalized = maxHp > 0f ? Mathf.Clamp01(hp / maxHp) : 1f;

            RuntimeManager.StudioSystem.setParameterByName("PlayerHealth", normalized);
        }
    }
}
