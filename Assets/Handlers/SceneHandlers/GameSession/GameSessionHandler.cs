using Assets.Handlers.FileHandlers;
using Assets.Scripts.Actions;
using Cysharp.Threading.Tasks;
using Entity.Controllers;
using GameplayActions;
using System;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class GameSessionHandler : SingletonMonoBehaviour<GameSessionHandler>
    {
        [Header("Player Settings")]
        [SerializeField] private EntityController _entityPrefab;
        private EntityController _playerController;
        public EntityController PlayerController => _playerController;
        public static event Action<EntityController> OnPlayerSpawned;
        public static event Action OnPlayerDespawned;

        private static (PositionData, Vector2) _respawnPointData;
        private static GameSessionData _data;

        public static GameSessionData Data
        {
            get => _data;
            set
            {
                if (_respawnPointData.Item1 == null)
                    _respawnPointData = (new PositionData(), value.respawnPosition);
                else _respawnPointData.Item2 = value.respawnPosition;
                _data = value;
            }
        }

        private readonly InteractionContext _spawnContext = new();

        public async UniTask SpawnPlayer()
        {
            if (Data == null)
            {
                Debug.LogWarning("[GameSessionHandler] Cannot spawn player: Data == null");
                return;
            }

            Vector2 spawnPosition = _respawnPointData.Item2;
            if (_playerController == null)
            {
                if (_entityPrefab == null)
                {
                    Debug.LogError("[GameSessionHandler] PlayerPrefab is not assigned in the Inspector!");
                    return;
                }

                _playerController = Instantiate(_entityPrefab, spawnPosition, Quaternion.identity);
                DontDestroyOnLoad(_playerController.gameObject);
            }
            else
            {
                _playerController.transform.position = spawnPosition;
                _playerController.gameObject.SetActive(true);
            }

            await _playerController.Setup(Data.entityDataContainer);
            _spawnContext.SetSource(_playerController.gameObject);
            ActionProvider.Position.Execute(_spawnContext, _respawnPointData.Item1, spawnPosition);

            OnPlayerSpawned?.Invoke(_playerController);
        }

        public async UniTask RespawnPlayer()
        {
            OnPlayerDespawned?.Invoke();

            if (_playerController != null)
            {
                Destroy(_playerController.gameObject);
                _playerController = null;
            }

            await SpawnPlayer();
        }

        private void OnDestroy() => OnPlayerDespawned?.Invoke();
    }
}