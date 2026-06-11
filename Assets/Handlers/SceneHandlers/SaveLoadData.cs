using System;
using Assets.DataContainers;
using Assets.Handlers.FileHandlers;
using Entity.Controllers.GenericController;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class SaveLoadData : SingletonMonoBehaviour<SaveLoadData>
    {
        public string FileName;
        public GameObject Player;

        private void Start()
        {
            try
            {
                string path = Application.streamingAssetsPath + "/Saves/" + FileName;
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
            var entityController = Player.GetComponent<EntityController>();
            entityController.Setup(data.EntityDataContainer);
        }
    }
}
