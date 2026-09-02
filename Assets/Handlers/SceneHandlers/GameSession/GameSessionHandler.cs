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
        [SerializeField] private EntityController playerPrefab;

        [HideInInspector] public EntityController playerController;

        public static event Action<EntityController> OnPlayerSpawned;
        public static event Action OnPlayerDespawned;
        public EntityController PlayerController => playerController;

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
            if (playerController == null)
            {
                if (playerPrefab == null)
                {
                    Debug.LogError("[GameSessionHandler] PlayerPrefab is not assigned in the Inspector!");
                    return;
                }

                playerController = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                DontDestroyOnLoad(playerController.gameObject);
            }
            else
            {
                playerController.transform.position = spawnPosition;
                playerController.gameObject.SetActive(true);
            }

            await playerController.Setup(Data.entityDataContainer);
            _spawnContext.SetSource(playerController.gameObject);
            ActionProvider.Position.Execute(_spawnContext, _respawnPointData.Item1, spawnPosition);

            OnPlayerSpawned?.Invoke(playerController);
        }

        public async UniTask RespawnPlayer()
        {
            OnPlayerDespawned?.Invoke();

            if (playerController != null)
            {
                Destroy(playerController.gameObject);
                playerController = null;
            }

            await SpawnPlayer();
        }

        private void OnDestroy() => OnPlayerDespawned?.Invoke();
    }
}