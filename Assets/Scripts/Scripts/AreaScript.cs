using System.Collections.Generic;
using Assets.Entity;
using Assets.Handlers.SceneHandlers;
using UnityEngine;

namespace Assets.Scripts.Scripts
{
    public class AreaScript : ScriptBase
    {
        [SerializeField] public List<MonoBehaviour> Scripts;
        private List<IScript> _scripts = new();
        private bool _isExecuted;

        public List<IScript> GetScripts() => _scripts;
        private void Awake()
        {
            foreach (MonoBehaviour script in Scripts)
            {
                if (script is IScript s)
                    _scripts.Add(s);
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
            if (_scripts.Count == 0)
            {
                Debug.LogWarning("Script list is empty!");
                return false;
            }
            foreach (IScript _script in _scripts)
            {
                _script.Execute(entityController);
            }
            _isExecuted = true;
            return true;
        }

        public override bool IsFinished(EntityController entityController) => true;

        public override bool IsExecuted(EntityController entityController) => _isExecuted;
    }
}