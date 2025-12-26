using System;
using Assets.DataContainers;
using Assets.Entity;
using Assets.Handlers.FileHandlers;
using Entity.Controllers.GenericController;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Handlers.SceneHandlers
{
    public class SaveLoadData : MonoBehaviour
    {
        public string FileName;
        public GameObject Player;
        public static SaveLoadData Instance { get; private set; }

        private void Start()
        {
            try
            {
                MakeSingleton();
                string path = Application.streamingAssetsPath + "/Saves/" + FileName;
                var data = LoadData(path);
                if (data == null) throw new Exception("Data file is not loaded");
                ExtractData(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public void MakeSingleton()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        private DataBundle LoadData(string path) => DataFileHandler.LoadFromJson<DataBundle>(path);

        private void ExtractData(DataBundle data)
        {
            var entityController = Player.GetComponent<EntityController>();
            entityController.Setup(data.EntityDataContainer);
        }
    }
}
