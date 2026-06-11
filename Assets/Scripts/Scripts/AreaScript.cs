using System.Collections.Generic;
using Assets.Handlers.SceneHandlers;
using Entity.Controllers.GenericController;
using UnityEngine;

namespace Scripts
{
    public class AreaScript : ScriptBase
    {
        [SerializeField] public List<MonoBehaviour> Scripts;
        private List<IScript> scripts = new();
        private bool isExecuted;

        public List<IScript> GetScripts() => scripts;
        private void Awake()
        {
            foreach (MonoBehaviour script in Scripts)
            {
                if (script is IScript s)
                    scripts.Add(s);
                else
                    Debug.LogWarning("Assigned scriptBehaviour does not implement IScript!");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var entityController = GameObjectHandler.GetEntityController(other);
            if (entityController) Execute(entityController);
        }

        public override bool Execute(EntityController entityController)
        {
            if (scripts.Count == 0)
            {
                Debug.LogWarning("Script list is empty!");
                return false;
            }
            foreach (IScript script in scripts)
            {
                script.Execute(entityController);
            }
            isExecuted = true;
            return true;
        }

        public override bool IsFinished(EntityController entityController) => true;
    }
}