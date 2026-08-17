using Assets.Handlers.SceneHandlers;
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
        public override bool Execute(Entity.Controllers.EntityController entityController)
        {
            if (GameObjectHandler.IsPlayer(entityController))
            {
                sceneController.GetComponent<SceneController>().NextLocation(LocationName);
                entityController.transform.position = Vector3.zero;
                return true;
            }
            return false;
        }

        public override bool IsFinished(Entity.Controllers.EntityController entityController) => false;
    }
}
