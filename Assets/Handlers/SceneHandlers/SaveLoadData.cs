using System;
using Assets.DataContainers;
using Assets.Handlers.FileHandlers;
using Entity.Controllers;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class SaveLoadData : SingletonMonoBehaviour<SaveLoadData>
    {
        public string fileName;
        private GameObject _player;

        protected override void Awake()
        {
            base.Awake();
            try
            {
                _player = GameObjectHandler.playerController.gameObject;
                string path = Application.streamingAssetsPath + "/Saves/" + fileName;
                var data = LoadData(path);
                if (data == null) throw new Exception("Data file is not loaded");
                ExtractData(data);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
            
        }

        private DataBundle LoadData(string path) => DataFileHandler.LoadFromJson<DataBundle>(path);

        private void ExtractData(DataBundle data)
        {
            var entityController = _player.GetComponent<EntityController>();
            entityController.Setup(data.EntityDataContainer);
        }
    }
}
