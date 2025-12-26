using Assets.Handlers.SceneHandlers;
using Entity.Controllers.GenericController;
using UnityEngine;

namespace Scripts
{
    public class NextLocationArea : ScriptBase
    {
        public string LocationName;
        private GameObject _sceneController;
        private void Awake()
        {
            _sceneController = GameObject.Find("Handlers");
            if (_sceneController == null) Debug.LogWarning("SceneController not found");
        }
        public override bool Execute(EntityController entityController)
        {
            if (entityController.isPlayer)
            {
                _sceneController.GetComponent<SceneController>().NextLocation(LocationName);
                entityController.transform.position = Vector3.zero;
                return true;
            }
            return false;
        }

        public override bool IsFinished(EntityController entityController) => false;

        public override bool IsExecuted(EntityController entityController) => false;
    }
}
