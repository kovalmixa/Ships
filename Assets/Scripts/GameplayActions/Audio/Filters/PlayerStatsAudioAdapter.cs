using Assets.Entity.Modifiers;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers;
using FMODUnity;
using UnityEngine;

namespace Assets.Scripts.GameplayActions.Audio.Filters
{
    public class PlayerStatsAudioAdapter : MonoBehaviour
    {
        [ParamRef]
        [SerializeField] private string _fmodParameterName = "PlayerHealth";

        private EntityController _playerEC;

        private void OnEnable()
        {
            GameSessionHandler.OnPlayerSpawned += AttachToPlayer;
            GameSessionHandler.OnPlayerDespawned += DetachFromPlayer;

            if (GameSessionHandler.Instance != null && GameSessionHandler.Instance.PlayerController != null)
                AttachToPlayer(GameSessionHandler.Instance.PlayerController);
        }

        private void OnDisable()
        {
            GameSessionHandler.OnPlayerSpawned -= AttachToPlayer;
            GameSessionHandler.OnPlayerDespawned -= DetachFromPlayer;

            DetachFromPlayer();
        }

        private void AttachToPlayer(EntityController newPlayer)
        {
            DetachFromPlayer();
            _playerEC = newPlayer;

            if (_playerEC != null && _playerEC.StatModController != null)
            {
                _playerEC.StatModController.OnChange += UpdateAudioParameter;
                UpdateAudioParameter();
            }
        }

        private void DetachFromPlayer()
        {
            if (_playerEC != null && _playerEC.StatModController != null)
                _playerEC.StatModController.OnChange -= UpdateAudioParameter;

            _playerEC = null;
            RuntimeManager.StudioSystem.setParameterByName(_fmodParameterName, 0f);
        }

        private void UpdateAudioParameter()
        {
            if (_playerEC == null || _playerEC.StatModController == null) return;

            float maxHp = _playerEC.GetTotalLifetimeStat(StatType.MaxHp);
            float currentHp = _playerEC.GetTotalLifetimeStat(StatType.Hp);
            float normalizedHp = maxHp > 0f ? Mathf.Clamp01(currentHp / maxHp) : 0f;
            RuntimeManager.StudioSystem.setParameterByName(_fmodParameterName, normalizedHp);
        }
    }
}