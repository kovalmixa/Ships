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
        public override bool Execute(EntityController entityController)
        {
            if (GameOb entityController.isPlayer)
            {
                sceneController.GetComponent<SceneController>().NextLocation(LocationName);
                entityController.transform.position = Vector3.zero;
                return true;
            }
            return false;
        }

        public override bool IsFinished(EntityController entityController) => false;
    }
}
