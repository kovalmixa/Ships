using Assets.Global.FileGlobal;
using Assets.Resources_moved.UI.GUI.Inventory.Player;
using Assets.Resources_moved.UI.GUI.Prefabs.Slot.Data;
using Assets.Scripts.Actions;
using Cysharp.Threading.Tasks;
using Entity.Controllers;
using GameplayActions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Global.SceneHandlers.GameSession
{
    public class GameSessionController : SingletonMonoBehaviour<GameSessionController>
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
            private set
            {
                if (value == null) return;

                if (_respawnPointData.Item1 == null)
                    _respawnPointData = (new PositionData(), value.respawnPosition);
                else
                    _respawnPointData.Item2 = value.respawnPosition;

                _data = value;
            }
        }

        private readonly List<GameSessionData> _saves = new();
        public IReadOnlyList<GameSessionData> Saves => _saves;

        private readonly InteractionContext _spawnContext = new();

        protected override void Awake()
        {
            base.Awake();
            LoadAllSaves();
        }

        #region Session Management & Orchestration

        public async UniTaskVoid StartGameSession(int saveIndex)
        {
            if (saveIndex < 0 || saveIndex >= _saves.Count)
            {
                Debug.LogError($"[GameSessionController] Неверный индекс сохранения: {saveIndex}");
                return;
            }

            var sessionData = _saves[saveIndex];
            await InitializeAndLaunchSession(sessionData);
        }

        private async UniTask InitializeAndLaunchSession(GameSessionData sessionData)
        {
            Data = sessionData;
            DistributeDataToServices(sessionData);

            if (SceneController.Instance != null)
                await SceneController.Instance.NextLocation(sessionData.locationName);
            else
                Debug.LogWarning("[GameSessionController] SceneController не найден, сцена не загружена через контроллер сцен.");

            await SpawnPlayer();
        }

        private void DistributeDataToServices(GameSessionData sessionData)
        {
            Dictionary<Vector2, InventoryItemData> inventoryDict = new Dictionary<Vector2, InventoryItemData>();

            if (sessionData.inventoryItems != null)
            {

            }

            if (PlayerInventoryController.Instance != null)
            {
                PlayerInventoryController.Instance.InitInventoryData(inventoryDict);
            }
            else
            {
                Debug.LogWarning("[GameSessionController] PlayerInventoryController не найден на сцене!");
            }
        }

        #endregion

        #region Save Loading

        private void LoadAllSaves()
        {
            _saves.Clear();
            string savesFolderPath = Path.Combine(Application.streamingAssetsPath, "Saves");

            var paths = FilesPathExtractor.GetFilePaths(new string[] { savesFolderPath });
            foreach (var path in paths)
            {
                try
                {
                    var save = DataFileHandler.LoadFromJson<GameSessionData>(path);
                    if (save != null)
                    {
                        _saves.Add(save);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GameSessionController] Ошибка загрузки сохранения из {path}: {ex.Message}");
                }
            }
        }

        #endregion

        #region Player Spawning

        public async UniTask SpawnPlayer()
        {
            if (Data == null)
            {
                Debug.LogWarning("[GameSessionController] Невозможно заспавнить игрока: Data == null");
                return;
            }

            Vector2 spawnPosition = _respawnPointData.Item2;

            if (_playerController == null)
            {
                if (_entityPrefab == null)
                {
                    Debug.LogError("[GameSessionController] PlayerPrefab не назначен в инспекторе!");
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

        #endregion

        private void OnDestroy() => OnPlayerDespawned?.Invoke();
    }
}