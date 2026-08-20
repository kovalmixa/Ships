using Assets.Handlers.SceneHandlers;
using Cysharp.Threading.Tasks;
using Entity.Controllers;
using UnityEngine;

namespace Scripts
{
    public class NextLocationArea : ScriptBase
    {
        public string LocationName;
        private GameObject sceneController;
        private void Awake()
        {
            sceneController = GameObject.Find("Handlers");
            if (sceneController == null) Debug.LogWarning("SceneController not found");
        }
        public override bool Execute(EntityController entityController)
        {
            if (GameObjectHandler.IsPlayer(entityController))
            {
                if (sceneController != null) 
                    sceneController.GetComponent<SceneController>().NextLocation(LocationName).Forget();
                return true;
            }
            return false;
        }

        public override bool IsFinished(EntityController entityController) => false;
    }
}
